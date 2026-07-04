// ============================================================
// JwtSettings.cs — Konfigurasi JWT
// ============================================================
// Class ini membaca konfigurasi JWT dari appsettings.json.
// Menggunakan Options Pattern — cara yang benar untuk membaca
// konfigurasi di ASP.NET Core.
//
// Mengapa Options Pattern?
//   - Strongly typed: tidak ada "magic string" untuk key config
//   - Validatable: bisa validasi konfigurasi saat startup
//   - Testable: mudah di-mock
//   - IntelliSense support
// ============================================================

namespace TesBackendNet.Authentication.Configurations;

/// <summary>
/// Konfigurasi JWT yang dibaca dari appsettings.json > "Jwt" section.
/// </summary>
public class JwtSettings
{
    // ── Nama Section di appsettings.json ─────────────────────
    // Digunakan untuk binding: builder.Services.Configure<JwtSettings>(...)
    public const string SectionName = "Jwt";

    /// <summary>
    /// Secret key untuk signing JWT.
    /// JANGAN hardcode di sini! Baca dari appsettings atau environment variable.
    /// Minimal 32 karakter untuk keamanan.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Issuer: siapa yang menerbitkan token.
    /// Biasanya nama aplikasi atau domain.
    /// Contoh: "TesBackendNet" atau "https://yourdomain.com"
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience: siapa yang menjadi target token.
    /// Biasanya nama client atau domain frontend.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Durasi Access Token dalam menit.
    /// Best practice: 15-60 menit.
    /// Default: 60 menit
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Durasi Refresh Token dalam hari.
    /// Best practice: 7-30 hari.
    /// Default: 7 hari
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}
