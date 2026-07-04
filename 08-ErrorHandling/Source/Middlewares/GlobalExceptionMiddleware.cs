// ============================================================
// Nama File: GlobalExceptionMiddleware.cs — Middleware Penangkap Error
// Folder: 08-ErrorHandling/Source/Middlewares/
// ============================================================
// 1. PENJELASAN FOLDER (ErrorHandling):
//    - Tujuan: Menyediakan manajemen kesalahan terpusat di tingkat aplikasi web.
//    - Kapan Digunakan: Saat memproses HTTP request untuk mencegah kebocoran stack trace internal ke klien.
//    - Hubungan: Menangkap exception dari Controller, Service, maupun Repository.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menjadi jaring pengaman (safety net) paling luar dalam pipeline HTTP ASP.NET Core.
//    - Mengapa Diperlukan: Jika terjadi error tak terduga, server tidak akan mengalami crash / mengembalikan halaman HTML IIS/Kestrel default yang merusak estetika REST API.
//    - Hubungan File: Terdaftar di Program.cs menggunakan app.UseMiddleware<GlobalExceptionMiddleware>().
//    - Jika Dihapus: Server akan mengembalikan HTTP 500 mentah tanpa struktur JSON yang seragam, membingungkan klien.
// ============================================================

using System.Text.Json;
using TesBackendNet.ErrorHandling.Exceptions;

namespace TesBackendNet.ErrorHandling.Middlewares;

/// <summary>
/// TUJUAN CLASS:
/// Menangkap exception secara global (Global Exception Handler) di dalam ASP.NET Core middleware pipeline.
/// 
/// ALASAN MENGGUNAKAN MIDDLEWARE UNTUK ERROR HANDLING:
/// Menggunakan middleware terpusat membebaskan kita dari keharusan membungkus setiap method Controller 
/// dengan blok try-catch yang berulang (boiler-plate code). Ini menerapkan prinsip DRY (Don't Repeat Yourself).
/// 
/// LIFECYCLE:
/// Singleton-like per request pipeline instance. Dibuat sekali saat aplikasi start, 
/// dan method InvokeAsync dipanggil secara berturut-turut untuk setiap HTTP Request yang masuk.
/// 
/// DEPENDENCY:
/// - RequestDelegate: Representasi middleware berikutnya dalam antrian pipeline.
/// - ILogger: Service pencatatan log (logging) bawaan .NET untuk menyimpan detail error.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>
    /// CONSTRUCTOR: Injeksi dependensi yang diperlukan middleware.
    /// </summary>
    /// <param name="next">Delegasi untuk melanjutkan request ke middleware berikutnya.</param>
    /// <param name="logger">Alat bantu untuk mencatat log ke konsol atau berkas.</param>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// FUNGSI METHOD: Mengeksekusi logika middleware per HTTP Request.
    /// PARAMETER: HttpContext (menyimpan detail request dan response HTTP saat ini).
    /// NILAI KEMBALIAN: Task (operasi asinkron tanpa nilai balik).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Middleware memanggil `await _next(context)` untuk meneruskan proses ke middleware berikutnya.
    /// 2. Jika middleware/controller setelahnya melempar Exception, eksekusi akan melompat ke blok `catch`.
    /// 3. Blok catch merekam pesan kesalahan lewat logger, lalu memanggil `HandleExceptionAsync` untuk menyusun response kustom.
    /// 
    /// BEST PRACTICE:
    /// Selalu bungkus `_next(context)` dalam blok try-catch di middleware terluar untuk menjamin ketahanan sistem.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Meneruskan request ke middleware berikutnya (contoh: Routing, Controller)
            await _next(context); 
        }
        catch (Exception ex)
        {
            // Mencatat detail error lengkap (termasuk Stack Trace) ke media logging secara aman (tidak bocor ke klien)
            _logger.LogError(ex, "Terjadi kesalahan tidak terduga di server: {Message}", ex.Message);
            
            // Mengubah exception menjadi JSON response yang terstruktur
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// FUNGSI METHOD: Mengonversi objek exception menjadi respon JSON yang seragam.
    /// PARAMETER: 
    ///  - HttpContext: Untuk memanipulasi header response, status code, dan menulis body response.
    ///  - Exception: Objek error yang ditangkap.
    /// 
    /// ALASAN IMPLEMENTASI (Mengapa begini?):
    /// Kita memetakan tipe Exception spesifik ke HTTP Status Code yang relevan:
    ///  - AppException (Custom) -> Status kustom yang ditentukan di exception tersebut (misal 400, 409).
    ///  - KeyNotFoundException -> HTTP 404 (Not Found).
    ///  - ArgumentException -> HTTP 400 (Bad Request).
    ///  - Exception umum lainnya -> HTTP 500 (Internal Server Error) demi menyembunyikan detail sensitif server.
    /// </summary>
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // ── Baris Kode Penting ───────────────────────────────────────
        // Menentukan content type sebagai JSON karena API tidak mengembalikan HTML.
        context.Response.ContentType = "application/json";

        var statusCode = StatusCodes.Status500InternalServerError; // Default fallback jika error tidak dikenali
        var message = "Terjadi kesalahan internal pada server.";    // Pesan aman untuk umum (Production-ready)

        // Polimorfisme Exception Handling
        if (exception is AppException appException)
        {
            // Jika error sengaja dilempar oleh logika bisnis kita (Custom Exception)
            statusCode = appException.StatusCode;
            message = appException.Message;
        }
        else if (exception is KeyNotFoundException)
        {
            // Jika data tidak ditemukan di database/dictionary
            statusCode = StatusCodes.Status404NotFound;
            message = exception.Message;
        }
        else if (exception is ArgumentException)
        {
            // Jika parameter input tidak sesuai aturan dasar
            statusCode = StatusCodes.Status400BadRequest;
            message = exception.Message;
        }

        // Menyetel HTTP Status Code resmi pada HTTP Response Header
        context.Response.StatusCode = statusCode;

        // Menyusun standard unified response envelope
        var response = new
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Timestamp = DateTime.UtcNow // Informasi waktu kejadian (sangat membantu saat debugging log)
        };

        // Konfigurasi JSON serializer agar konsisten menggunakan format camelCase
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        
        // Menuliskan response body secara asinkron ke dalam stream
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
