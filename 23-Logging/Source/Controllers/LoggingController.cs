// ============================================================
// Nama File: LoggingController.cs — Controller Demo Logging (Level, Structured, Exception)
// Folder: 23-Logging/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Logging):
//    - Tujuan: Mendemonstrasikan sistem pencatatan log yang benar di ASP.NET Core menggunakan Microsoft.Extensions.Logging
//      dan Serilog sebagai provider.
//    - Kapan Digunakan: Logging digunakan di setiap komponen aplikasi — controller, service, middleware — untuk
//      membantu monitoring, debugging, dan audit trail di lingkungan produksi.
//    - Hubungan: ILogger<T> disuntikkan oleh DI Container. Konfigurasi sink (tujuan log) ditentukan di Program.cs dan appsettings.json.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyajikan 3 demonstrasi: level log, structured vs string interpolation, dan logging exception.
//    - Mengapa Diperlukan: Pengetahuan logging yang benar adalah kompetensi wajib backend developer di lingkungan produksi.
//    - Jika Dihapus: Tidak ada demonstrasi logging fungsional pada modul ini.
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.Logging.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller demonstrasi yang memperlihatkan 3 praktik penting dalam logging aplikasi backend.
/// </summary>
[ApiController]
[Route("api/logging")]
public class LoggingController : ControllerBase
{
    private readonly ILogger<LoggingController> _logger;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan ILogger<T> dari DI Container.
    /// 
    /// MENGAPA GENERICS ILogger<LoggingController>?
    /// Parameter generik T menentukan nama "category" (sumber) dari log entry.
    /// Saat log ditulis, system log akan menambahkan namespace lengkap kelas ini sebagai kategori,
    /// sehingga mudah disaring (filter) berdasarkan sumber di dashboard log terpusat.
    /// </summary>
    public LoggingController(ILogger<LoggingController> logger)
    {
        _logger = logger;
    }

    // ================================================================
    // 1. DEMO BERBAGAI LEVEL LOG
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Memicu semua level log dari paling rendah hingga paling kritis.
    /// 
    /// HIERARKI LEVEL LOG (dari paling detail ke paling kritis):
    /// 1. Trace (0) — Level paling verbose. Catatan sangat detail seperti nilai variabel di setiap langkah.
    ///                Dimatikan di produksi (terlalu banyak data).
    /// 2. Debug (1) — Informasi debugging: nilai variabel, flow masuk/keluar method.
    ///                Aktif di Development, mati di Production.
    /// 3. Information (2) — Catatan alur normal aplikasi: "User berhasil login", "Order dibuat".
    ///                      Ini adalah level log standar di produksi.
    /// 4. Warning (3) — Kondisi tidak ideal tapi tidak sampai error: retry ke-3, response lambat, konfigurasi default.
    /// 5. Error (4) — Kesalahan yang ter-handle: exception yang ditangkap, request gagal validasi.
    /// 6. Critical (5) — Masalah fatal yang menghentikan aplikasi: database tidak bisa terkoneksi, memory penuh.
    /// 
    /// Konfigurasi level minimum ada di `appsettings.json` → `"Logging": { "LogLevel": { "Default": "Information" } }`.
    /// </summary>
    [HttpGet("levels")]
    public IActionResult LogAllLevels()
    {
        _logger.LogTrace       ("Ini adalah TRACE log (Detail level rendah/development).");
        _logger.LogDebug       ("Ini adalah DEBUG log (Informasi debugging).");
        _logger.LogInformation ("Ini adalah INFORMATION log (Alur normal aplikasi).");
        _logger.LogWarning     ("Ini adalah WARNING log (Kondisi tidak ideal/janggal tapi bukan error).");
        _logger.LogError       ("Ini adalah ERROR log (Kesalahan ter-handle).");
        _logger.LogCritical    ("Ini adalah CRITICAL log (Masalah fatal/system crash).");

        return Ok(new { Message = "Semua level log berhasil dipicu! Silakan cek konsol atau file log." });
    }

    // ================================================================
    // 2. STRUCTURED LOGGING VS STRING INTERPOLATION
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Memperlihatkan perbedaan antara structured logging (direkomendasikan) dan string interpolation (tidak disarankan).
    /// PARAMETER: userId (ID pengguna), role (peran pengguna).
    /// 
    /// MENGAPA STRUCTURED LOGGING LEBIH BAIK:
    /// 
    /// ❌ String Interpolation (`$"User {userId}..."`):
    ///   Log disimpan sebagai satu string datar. Tidak dapat di-query per field.
    ///   Di Elasticsearch/Seq, pencarian `UserId = 123` tidak bisa dilakukan.
    /// 
    /// ✅ Structured Logging (`"User {UserId}...", userId`):
    ///   Log disimpan sebagai dokumen terstruktur: `{ "UserId": 123, "UserRole": "Admin", "Timestamp": "..." }`.
    ///   Dapat difilter, diagregasi, dan divisualisasikan di Kibana/Seq/Grafana.
    ///   Contoh query Kibana: `UserId: 123 AND UserRole: "Admin"`.
    /// 
    /// BARIS KODE PENTING:
    /// `_logger.LogInformation("User {UserId} dengan role {UserRole}.", userId, role);`
    ///   - `{UserId}` dan `{UserRole}` adalah placeholder yang diberi nama (named placeholders).
    ///   - Nilai `userId` dan `role` diteruskan sebagai argument ke-2 dan ke-3 secara urutan.
    ///   - Serilog/Seq akan menyimpan keduanya sebagai field terpisah di dokumen log.
    /// </summary>
    [HttpGet("structured")]
    public IActionResult StructuredLoggingDemo([FromQuery] int userId, [FromQuery] string role)
    {
        // ❌ TIDAK DISARANKAN: String datar yang tidak dapat di-query per field di log server
        _logger.LogInformation($"[Interpolated] User {userId} dengan role {role} masuk sistem pada {DateTime.Now}");

        // ✅ DISARANKAN: Structured logging — data tersimpan sebagai fields terpisah
        _logger.LogInformation("[Structured] User {UserId} dengan role {UserRole} masuk sistem.", userId, role);

        return Ok(new { Message = "Perbandingan log terkirim ke server log." });
    }

    // ================================================================
    // 3. LOG EXCEPTION DENGAN STACK TRACE LENGKAP
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mendemonstrasikan cara yang benar untuk mencatat exception beserta stack trace-nya.
    /// 
    /// CARA YANG BENAR (`_logger.LogError(ex, "pesan")`):
    /// - Parameter pertama adalah objek Exception (`ex`), bukan string.
    /// - Serilog dan penyedia log lainnya akan mengekstrak: tipe exception, pesan, dan full stack trace.
    /// - Hasil di log server: structured JSON dengan field `ExceptionType`, `ExceptionMessage`, `StackTrace`.
    /// 
    /// CARA YANG SALAH (`_logger.LogError($"Error: {ex.Message}")`):
    /// - Hanya mencatat pesan singkat, kehilangan stack trace.
    /// - Sangat sulit untuk debug karena tidak tahu baris mana yang menyebabkan error.
    /// </summary>
    [HttpGet("exception")]
    public IActionResult LogExceptionDemo()
    {
        try
        {
            // Simulasi bug runtime: pembagian dengan nol
            int denominator = 0;
            int result = 100 / denominator;  // Melempar DivideByZeroException di sini
            return Ok(result);
        }
        catch (DivideByZeroException ex)
        {
            // Cara BENAR: Kirim objek Exception sebagai parameter pertama → stack trace tercatat
            _logger.LogError(ex, "Terjadi kesalahan fatal saat melakukan perhitungan pembagian.");
            
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success   = false,
                Message   = "Perhitungan gagal.",
                ErrorType = ex.GetType().Name  // Menampilkan tipe exception ke klien (boleh di produksi)
            });
        }
    }
}
