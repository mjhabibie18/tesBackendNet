// ============================================================
// TokenService.cs — Service untuk Generate & Validate JWT
// ============================================================
// TokenService bertanggung jawab untuk:
//   1. Generate Access Token (JWT)
//   2. Generate Refresh Token (random string)
//   3. Validate Access Token
//   4. Extract claims dari token
//
// Mengapa dipisah dari AuthService?
//   - Single Responsibility: TokenService hanya urusan token
//   - Reusable: bisa dipakai di Service lain
//   - Testable: mudah di-unit test terpisah
// ============================================================

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TesBackendNet.Authentication.Configurations;
using TesBackendNet.Authentication.Models;

namespace TesBackendNet.Authentication.Services;

/// <summary>Interface untuk operasi token JWT dan Refresh Token.</summary>
public interface ITokenService
{
    /// <summary>Generate JWT Access Token untuk user yang sudah login.</summary>
    string GenerateAccessToken(User user);

    /// <summary>Generate Refresh Token sebagai random secure string.</summary>
    string GenerateRefreshToken();

    /// <summary>Extract ClaimsPrincipal dari token (bahkan yang sudah expire).</summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

/// <summary>
/// Implementasi TokenService menggunakan System.IdentityModel.Tokens.Jwt.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;

    // IOptions<T>: cara membaca konfigurasi typed dari appsettings.json
    // Alternatif: IOptionsMonitor<T> jika perlu hot-reload config
    public TokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    // ── GenerateAccessToken ───────────────────────────────────
    public string GenerateAccessToken(User user)
    {
        // ── Security Key ──────────────────────────────────────
        // SymmetricSecurityKey: menggunakan satu key untuk sign dan verify
        // Konversi secret key string → bytes → SymmetricSecurityKey
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

        // ── Signing Credentials ───────────────────────────────
        // SigningCredentials: kombinasi key + algoritma signing
        // SecurityAlgorithms.HmacSha256: HMAC-SHA256 (rekomendasi untuk JWT)
        //   - HS256: symmetric (satu key)
        //   - RS256: asymmetric (public/private key) — untuk multi-service
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // ── Claims ────────────────────────────────────────────
        // Claims = informasi tentang user yang di-embed dalam token
        // Client bisa decode payload untuk baca claims (TAPI tidak bisa manipulasi)
        //
        // Jenis Claims:
        //   - Registered claims: standard claims dari JWT spec (sub, iss, aud, exp, iat)
        //   - Public claims: nama yang terdaftar di IANA
        //   - Private claims: custom claims untuk aplikasi kita
        var claims = new List<Claim>
        {
            // sub (Subject): identitas unik user, biasanya user ID
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),

            // jti (JWT ID): unique identifier per token (untuk revocation)
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // email: email user
            new(JwtRegisteredClaimNames.Email, user.Email),

            // iat (Issued At): kapan token dibuat
            new(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),

            // Custom claims
            new("userId", user.Id.ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName ?? ""),

            // Role claim — penting untuk Authorization!
            // ClaimTypes.Role: claim khusus untuk role, ASP.NET Core mengenali ini
            new(ClaimTypes.Role, user.Role),
        };

        // ── Create Token ──────────────────────────────────────
        var token = new JwtSecurityToken(
            issuer:             _jwtSettings.Issuer,           // dari appsettings
            audience:           _jwtSettings.Audience,          // dari appsettings
            claims:             claims,
            notBefore:          DateTime.UtcNow,                // valid mulai sekarang
            expires:            DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        // ── Serialize Token ───────────────────────────────────
        // JwtSecurityTokenHandler: serialize JwtSecurityToken → string JWT
        // Format: "eyJ...header.eyJ...payload.signature"
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ── GenerateRefreshToken ──────────────────────────────────
    public string GenerateRefreshToken()
    {
        // Gunakan cryptographically secure random number generator
        // JANGAN gunakan Random() biasa untuk security purposes!
        //
        // RandomNumberGenerator.GetBytes(64): generate 64 random bytes
        // Convert ke Base64 URL-safe string (88 karakter)
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    // ── GetPrincipalFromExpiredToken ──────────────────────────
    // Digunakan saat refresh: ambil user info dari access token yang sudah expire
    // Tanpa validasi expiry (karena token memang sudah expire = normal)
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = false, // ← PENTING: jangan validasi expiry
            ValidateIssuerSigningKey = true,
            ValidIssuer              = _jwtSettings.Issuer,
            ValidAudience            = _jwtSettings.Audience,
            IssuerSigningKey         = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(_jwtSettings.SecretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            // ValidateToken: verify signature + extract claims
            var principal = tokenHandler.ValidateToken(
                token, tokenValidationParameters, out var securityToken);

            // Pastikan token menggunakan algoritma yang benar
            if (securityToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null; // Token tidak valid (bukan format JWT yang benar)
        }
    }
}
