// ============================================================
// JwtSettings.cs — DTO Options Pattern untuk JWT Settings
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace TesBackendNet.EnvironmentDemo.Options;

public class JwtSettings
{
    // Menggunakan data annotations untuk memvalidasi konfigurasi saat startup (fail-fast)
    [Required]
    [MinLength(32, ErrorMessage = "JwtSettings:SecretKey minimal berdurasi 32 karakter demi keamanan!")]
    public string SecretKey { get; set; } = string.Empty;

    [Range(1, 30, ErrorMessage = "JwtSettings:ExpiryDays harus bernilai antara 1 sampai 30 hari.")]
    public int ExpiryDays { get; set; }
}
