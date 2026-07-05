// ============================================================
// Nama File: JwtSettings.cs — Options Class untuk Konfigurasi JWT via Options Pattern
// Folder: 24-Environment/Source/Options/
// ============================================================
// 1. PENJELASAN FOLDER (Environment/Options):
//    - Tujuan: Menyimpan kelas POCO (Plain Old C# Object) yang menjadi representasi kuat-tipe (strongly-typed)
//      dari bagian konfigurasi di appsettings.json.
//    - Kapan Digunakan: Saat konfigurasi memiliki banyak nilai terkait yang perlu dikelompokkan dan divalidasi.
//    - Hubungan: Kelas ini diikat (bound) ke seksi "JwtSettings" di appsettings.json oleh Program.cs melalui
//      `builder.Services.AddOptions<JwtSettings>().BindConfiguration("JwtSettings").ValidateDataAnnotations()`.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendefinisikan skema konfigurasi JWT beserta aturan validasinya menggunakan Data Annotations.
//    - Mengapa Diperlukan: Tanpa Options Pattern, konfigurasi diakses sebagai string mentah (`_config["JwtSettings:SecretKey"]`)
//      tanpa validasi, tanpa IntelliSense, dan rawan typo.
//    - Hubungan File: Diinjeksikan ke ConfigController.cs via IOptions<JwtSettings>.
//    - Jika Dihapus: Akses konfigurasi JWT menjadi tidak terstruktur dan tidak ada validasi saat startup.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace TesBackendNet.EnvironmentDemo.Options;

/// <summary>
/// TUJUAN CLASS:
/// Kelas representasi kuat-tipe (strongly-typed POCO) untuk konfigurasi JWT yang dibaca dari appsettings.json.
/// 
/// KEUNTUNGAN OPTIONS PATTERN DIBANDING IConfiguration LANGSUNG:
/// 1. IntelliSense: Developer mendapat autocomplete saat mengakses properti konfigurasi.
/// 2. Validasi: Menggunakan Data Annotations untuk memvalidasi nilai konfigurasi saat aplikasi startup.
///    Jika SecretKey kosong, aplikasi langsung gagal start (Fail-Fast) daripada gagal diam-diam saat runtime.
/// 3. Refactoring Safe: Jika nama property berubah, compiler langsung memberi peringatan.
/// 4. Testability: Class POCO mudah diinstansiasi dan di-mock dalam unit test.
/// 
/// POLA DESAIN: Options Pattern (Microsoft.Extensions.Options).
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// FUNGSI PROPERTY: Kunci rahasia yang digunakan untuk menandatangani (signing) JWT token.
    /// 
    /// ALASAN VALIDASI [Required]:
    /// Jika SecretKey tidak dikonfigurasi, JWT tidak dapat dibuat. Aplikasi harus gagal start (fail-fast)
    /// daripada mengirimkan token tidak aman atau mengalami error runtime yang membingungkan.
    /// 
    /// ALASAN [MinLength(32)]:
    /// Standar keamanan JWT (HMAC-SHA256) mengharuskan kunci rahasia minimal 256-bit (32 karakter ASCII).
    /// Kunci yang lebih pendek rentan terhadap brute-force attack.
    /// </summary>
    [Required]
    [MinLength(32, ErrorMessage = "JwtSettings:SecretKey minimal berdurasi 32 karakter demi keamanan!")]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Durasi masa berlaku JWT access token dalam jumlah hari.
    /// 
    /// ALASAN VALIDASI [Range(1, 30)]:
    /// Token yang tidak pernah kedaluwarsa (tanpa batas) adalah risiko keamanan serius.
    /// Token yang kedaluwarsa terlalu cepat (misal: 0 hari) membuat pengguna tidak bisa login.
    /// Range 1-30 hari memaksa developer memikirkan kebijakan expiry yang wajar.
    /// </summary>
    [Range(1, 30, ErrorMessage = "JwtSettings:ExpiryDays harus bernilai antara 1 sampai 30 hari.")]
    public int ExpiryDays { get; set; }
}
