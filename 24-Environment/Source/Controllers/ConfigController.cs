// ============================================================
// Nama File: ConfigController.cs — Controller Demo Environment & Konfigurasi
// Folder: 24-Environment/Source/Controllers/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menampilkan informasi konfigurasi aktif berdasarkan environment yang sedang berjalan.
//    - Mengapa Diperlukan: Membuktikan bahwa appsettings.Development.json, appsettings.Staging.json, dan appsettings.json
//      menghasilkan nilai konfigurasi yang berbeda berdasarkan environment aktif (ASPNETCORE_ENVIRONMENT).
//    - Hubungan File: Mengonsumsi JwtSettings.cs via IOptions<JwtSettings> dan IConfiguration untuk akses langsung.
//    - Jika Dihapus: Tidak ada endpoint untuk memverifikasi konfigurasi per-environment secara fungsional.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TesBackendNet.EnvironmentDemo.Options;

namespace TesBackendNet.EnvironmentDemo.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang menampilkan informasi konfigurasi yang sedang aktif untuk verifikasi per-environment.
/// 
/// DEPENDENCY YANG DIGUNAKAN:
/// 1. IWebHostEnvironment: Memberikan informasi environment aktif (Development, Staging, Production).
/// 2. IConfiguration: Akses langsung ke semua nilai konfigurasi via key string (kurang type-safe).
/// 3. IOptions<JwtSettings>: Akses konfigurasi JWT yang strongly-typed dan sudah tervalidasi.
/// </summary>
[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly IWebHostEnvironment  _env;
    private readonly IConfiguration      _config;
    private readonly JwtSettings         _jwtSettings;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan tiga dependensi konfigurasi yang berbeda untuk demonstrasi perbedaan pendekatannya.
    /// 
    /// PENJELASAN `IOptions<JwtSettings>.Value`:
    /// `IOptions<T>` adalah wrapper. Properti `.Value` memberikan akses ke objek JwtSettings yang sudah 
    /// terikat dengan nilai dari appsettings.json dan telah melewati validasi Data Annotations.
    /// </summary>
    public ConfigController(IWebHostEnvironment env, IConfiguration config, IOptions<JwtSettings> jwtSettings)
    {
        _env         = env;
        _config      = config;
        _jwtSettings = jwtSettings.Value; // Ambil objek JwtSettings yang sudah tervalidasi
    }

    /// <summary>
    /// FUNGSI METHOD: Mengembalikan informasi konfigurasi aktif berdasarkan environment saat ini.
    /// 
    /// KONSEP ENVIRONMENT (ASPNETCORE_ENVIRONMENT):
    /// - "Development": Digunakan di mesin developer. Config dari appsettings.Development.json.
    /// - "Staging": Digunakan di server pre-production. Config dari appsettings.Staging.json.
    /// - "Production": Digunakan di server produksi. Config dari appsettings.json (default).
    /// 
    /// ASP.NET Core secara otomatis menggabungkan (merge) appsettings.json dengan appsettings.{Environment}.json.
    /// Nilai di file environment-spesifik akan menimpa (override) nilai di appsettings.json.
    /// 
    /// KEAMANAN PENTING:
    /// `_jwtSettings.SecretKey.Substring(0, 5) + "...[MASKED]"` — Hanya menampilkan 5 karakter pertama.
    /// JANGAN PERNAH mengekspos nilai secret key lengkap melalui endpoint API, bahkan di Development.
    /// Secret key yang bocor memungkinkan penyerang memalsukan JWT token apa pun.
    /// </summary>
    [HttpGet("info")]
    public IActionResult GetConfigInfo()
    {
        return Ok(new
        {
            // Nama environment yang sedang berjalan (Development/Staging/Production)
            ActiveEnvironment = _env.EnvironmentName,

            AppFeatures = new
            {
                // IConfiguration["key:subkey"] — akses langsung via string key (rentan typo)
                EnablePremium = _config.GetValue<bool>("AppFeatures:EnablePremiumFeatures"),
                Gateway       = _config["AppFeatures:PaymentGateway"]
            },

            JwtSettings = new
            {
                // SENSOR secret key: hanya tampilkan 5 karakter pertama + mask untuk alasan keamanan
                MaskedSecretKey = _jwtSettings.SecretKey.Substring(0, 5) + "...[MASKED]",
                _jwtSettings.ExpiryDays
            }
        });
    }
}
