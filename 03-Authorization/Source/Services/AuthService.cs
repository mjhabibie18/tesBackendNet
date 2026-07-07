// ============================================================
// AuthService.cs — Business Logic Authentication
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.Authorization.Data;
using TesBackendNet.Authorization.DTO;
using TesBackendNet.Authorization.Models;

namespace TesBackendNet.Authorization.Services;

public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto, string? ipAddress = null);
    Task<AuthTokenDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task LogoutAsync(string refreshToken);
    Task<UserResponseDto?> GetProfileAsync(int userId);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;
    private readonly Configurations.JwtSettings _jwtSettings;

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        ILogger<AuthService> logger,
        Microsoft.Extensions.Options.IOptions<Configurations.JwtSettings> jwtSettings)
    {
        _context     = context;
        _tokenService = tokenService;
        _logger      = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
    {
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailExists)
            throw new InvalidOperationException($"Email '{dto.Email}' sudah terdaftar.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 11);

        var user = new User
        {
            Email        = dto.Email.ToLower().Trim(),
            PasswordHash = passwordHash,
            FirstName    = dto.FirstName.Trim(),
            LastName     = dto.LastName?.Trim(),
            // jika menggunakan hcak kecil untuk testing, gunakan [MODIFIKASI UNTUK TESTING]: Jika email mengandung 'admin', jadikan dia Admin.
            //Role         = dto.Email.ToLower().Contains("admin") ? "Admin" : "User", 
            Role         = "User", // Selalu assign 'User' untuk pendaftaran mandiri (security constraint)
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User baru terdaftar: {Email}", user.Email);

        return MapToUserDto(user);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, string? ipAddress = null)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Email atau password salah.");

        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!passwordValid)
            throw new UnauthorizedAccessException("Email atau password salah.");

        var accessToken  = _tokenService.GenerateAccessToken(user);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            Token     = refreshTokenStr,
            UserId    = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
        _context.RefreshTokens.Add(refreshToken);

        user.LastLoginAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User login berhasil: {Email}", user.Email);

        return new LoginResponseDto
        {
            User   = MapToUserDto(user),
            Tokens = new AuthTokenDto
            {
                AccessToken       = accessToken,
                RefreshToken      = refreshTokenStr,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                RefreshTokenExpiry = refreshToken.ExpiresAt
            }
        };
    }

    public async Task<AuthTokenDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
    {
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null || !tokenEntity.IsActive)
            throw new UnauthorizedAccessException("Refresh token tidak valid atau sudah expire.");

        if (!tokenEntity.User.IsActive)
            throw new UnauthorizedAccessException("Akun user tidak aktif.");

        tokenEntity.IsRevoked = true;

        var newRefreshTokenStr = _tokenService.GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            Token       = newRefreshTokenStr,
            UserId      = tokenEntity.UserId,
            ExpiresAt   = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt   = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
        _context.RefreshTokens.Add(newRefreshToken);

        var newAccessToken = _tokenService.GenerateAccessToken(tokenEntity.User);

        await _context.SaveChangesAsync();

        return new AuthTokenDto
        {
            AccessToken        = newAccessToken,
            RefreshToken       = newRefreshTokenStr,
            AccessTokenExpiry  = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenExpiry = newRefreshToken.ExpiresAt
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null) return;

        tokenEntity.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    public async Task<UserResponseDto?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToUserDto(user);
    }

    private static UserResponseDto MapToUserDto(User user) => new()
    {
        Id          = user.Id,
        Email       = user.Email,
        FirstName   = user.FirstName,
        LastName    = user.LastName,
        Role        = user.Role,
        CreatedAt   = user.CreatedAt,
        LastLoginAt = user.LastLoginAt
    };
}

public class LoginResponseDto
{
    public UserResponseDto User { get; set; } = null!;
    public AuthTokenDto Tokens { get; set; } = null!;
}
