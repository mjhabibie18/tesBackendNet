// ============================================================
// AuthService.cs — Business Logic Authentication
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.Authentication.Data;
using TesBackendNet.Authentication.DTO;
using TesBackendNet.Authentication.Models;

namespace TesBackendNet.Authentication.Services;

/// <summary>Interface untuk operasi autentikasi.</summary>
public interface IAuthService
{
    Task<UserResponseDto> RegisterAsync(RegisterDto dto);
    Task<LoginResponseDto> LoginAsync(LoginDto dto, string? ipAddress = null);
    Task<AuthTokenDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null);
    Task LogoutAsync(string refreshToken);
    Task<UserResponseDto?> GetProfileAsync(int userId);
}

/// <summary>
/// Implementasi business logic autentikasi.
/// Menggabungkan TokenService + bcrypt + database.
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    // JwtSettings untuk baca expiry duration
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

    // ── Register ──────────────────────────────────────────────
    /// <summary>
    /// Mendaftarkan user baru.
    ///
    /// Flow:
    /// 1. Validasi email belum terdaftar
    /// 2. Hash password dengan bcrypt
    /// 3. Simpan user ke database
    /// 4. Return user info (tanpa password)
    /// </summary>
    public async Task<UserResponseDto> RegisterAsync(RegisterDto dto)
    {
        // ── Cek email sudah terdaftar ─────────────────────────
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        if (emailExists)
            throw new InvalidOperationException($"Email '{dto.Email}' sudah terdaftar.");

        // ── Hash Password ─────────────────────────────────────
        // BCrypt.HashPassword():
        //   - Parameter 1: password plain text
        //   - Parameter 2: work factor (cost) — default 11
        //     Semakin tinggi = semakin lambat = semakin aman
        //     10 = ~100ms, 11 = ~300ms, 12 = ~600ms
        //   - Secara otomatis generate salt berbeda setiap kali dipanggil
        //     (dua hash dari password yang sama akan berbeda!)
        //
        // JANGAN simpan plain text! JANGAN pakai MD5/SHA1!
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, workFactor: 11);

        // ── Buat User ─────────────────────────────────────────
        var user = new User
        {
            Email        = dto.Email.ToLower().Trim(),
            PasswordHash = passwordHash,
            FirstName    = dto.FirstName.Trim(),
            LastName     = dto.LastName?.Trim(),
            Role         = "User",          // Default role saat register
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User baru terdaftar: {Email}", user.Email);

        return MapToUserDto(user);
    }

    // ── Login ─────────────────────────────────────────────────
    /// <summary>
    /// Login user dan return Access Token + Refresh Token.
    ///
    /// Flow:
    /// 1. Cari user berdasarkan email
    /// 2. Verify password dengan bcrypt
    /// 3. Generate Access Token (JWT)
    /// 4. Generate Refresh Token dan simpan ke database
    /// 5. Update last login
    /// 6. Return tokens
    /// </summary>
    public async Task<LoginResponseDto> LoginAsync(LoginDto dto, string? ipAddress = null)
    {
        // ── Cari User ─────────────────────────────────────────
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

        // ── Security Best Practice ────────────────────────────
        // JANGAN tampilkan pesan spesifik "email tidak ditemukan" atau "password salah"
        // karena bisa dipakai untuk enumerate valid emails (username enumeration attack)
        // Gunakan pesan generic: "Email atau password salah"
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Email atau password salah.");

        // ── Verify Password ───────────────────────────────────
        // BCrypt.Verify():
        //   - Parameter 1: plain text password dari user
        //   - Parameter 2: hash yang tersimpan di database
        //   - Return: true jika match, false jika tidak
        //
        // BCrypt extract salt dari hash, hash ulang password input,
        // bandingkan hasilnya. Aman dari timing attack.
        var passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        // Pesan sama: tidak expose apakah email atau password yang salah
        if (!passwordValid)
            throw new UnauthorizedAccessException("Email atau password salah.");

        // ── Generate Tokens ───────────────────────────────────
        var accessToken  = _tokenService.GenerateAccessToken(user);
        var refreshTokenStr = _tokenService.GenerateRefreshToken();

        // ── Simpan Refresh Token ke Database ──────────────────
        // Refresh Token disimpan di database agar bisa di-revoke
        var refreshToken = new RefreshToken
        {
            Token     = refreshTokenStr,
            UserId    = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedAt = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };
        _context.RefreshTokens.Add(refreshToken);

        // ── Update Last Login ─────────────────────────────────
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

    // ── RefreshToken ──────────────────────────────────────────
    /// <summary>
    /// Refresh Access Token menggunakan Refresh Token.
    ///
    /// Flow:
    /// 1. Cari Refresh Token di database
    /// 2. Validasi: aktif? belum expire? belum di-revoke?
    /// 3. Generate Access Token baru
    /// 4. Rotate Refresh Token (revoke lama, buat baru)
    /// 5. Return tokens baru
    /// </summary>
    public async Task<AuthTokenDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null)
    {
        // Cari refresh token di database, include user-nya
        var tokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        // Validasi token
        if (tokenEntity == null || !tokenEntity.IsActive)
            throw new UnauthorizedAccessException("Refresh token tidak valid atau sudah expire.");

        if (!tokenEntity.User.IsActive)
            throw new UnauthorizedAccessException("Akun user tidak aktif.");

        // ── Rotate Refresh Token ──────────────────────────────
        // Revoke token lama
        tokenEntity.IsRevoked = true;

        // Buat refresh token baru
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

        // Generate Access Token baru
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

    // ── Logout ────────────────────────────────────────────────
    public async Task LogoutAsync(string refreshToken)
    {
        var tokenEntity = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (tokenEntity == null) return; // Token tidak ada = sudah logout

        tokenEntity.IsRevoked = true;
        await _context.SaveChangesAsync();
    }

    // ── GetProfile ────────────────────────────────────────────
    public async Task<UserResponseDto?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user == null ? null : MapToUserDto(user);
    }

    // ── Helper: Map Entity → DTO ──────────────────────────────
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
