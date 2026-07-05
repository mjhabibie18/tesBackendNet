// ============================================================
// Nama File: RequestLoggingMiddleware.cs — Custom HTTP Request/Response Logging Middleware
// Folder: 16-Framework/Source/Middlewares/
// ============================================================
// 1. PENJELASAN FOLDER (Framework/Middlewares):
//    - Tujuan: Menyimpan komponen middleware kustom yang memproses setiap HTTP request/response.
//    - Kapan Digunakan: Saat dibutuhkan logika yang harus dieksekusi pada setiap request sebelum atau sesudah controller dipanggil.
//    - Hubungan: Middleware didaftarkan di Program.cs menggunakan app.UseMiddleware<T>() dan beroperasi sebagai komponen berurutan (pipeline).
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mencatat (log) setiap HTTP request yang masuk dan response yang keluar beserta durasi pemrosesannya.
//    - Mengapa Diperlukan: Membantu debugging, audit, dan monitoring performa endpoint API.
//    - Hubungan File: Didaftarkan di Program.cs dan membutuhkan ILogger dari DI Container untuk pencatatan log.
//    - Jika Dihapus: Log per-request tidak akan tercatat secara terpusat, monitoring API menjadi buta.
// ============================================================

using System.Diagnostics;

namespace TesBackendNet.Framework.Middlewares;

/// <summary>
/// TUJUAN CLASS:
/// Middleware kustom yang bertugas mencatat detail setiap HTTP request yang masuk dan response yang dikembalikan.
/// 
/// CARA KERJA MIDDLEWARE PIPELINE (ASP.NET Core):
/// Middleware disusun sebagai rantai (chain). Setiap middleware memutuskan apakah akan meneruskan request ke 
/// middleware berikutnya (`await _next(context)`) atau langsung menghentikan pipeline (short-circuit).
/// 
/// Representasi visual pipeline:
///   [Request Masuk]
///       → RequestLoggingMiddleware.InvokeAsync()
///           → [Middleware berikutnya: Auth, Routing, dll.]
///               → Controller Action (berjalan di sini)
///           ← [Middleware berikutnya kembali]
///       ← RequestLoggingMiddleware (mencatat response)
///   [Response Keluar]
/// 
/// DEPENDENCY:
/// - RequestDelegate: Fungsi yang merepresentasikan middleware berikutnya dalam rantai.
/// - ILogger: Service pencatatan log dari .NET Logging framework.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan dependensi yang diperlukan middleware.
    /// </summary>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    /// <summary>
    /// FUNGSI METHOD: Dieksekusi untuk setiap HTTP request yang melewati middleware ini.
    /// PARAMETER: HttpContext — berisi semua informasi tentang request dan response HTTP saat ini.
    /// 
    /// ALUR EKSEKUSI (Pola Before-After):
    /// 1. **BEFORE**: Catat detail request yang masuk dan mulai stopwatch pengukur waktu.
    /// 2. **PASS THROUGH**: `await _next(context)` — meneruskan ke middleware/controller berikutnya.
    ///    Eksekusi ditangguhkan di sini hingga seluruh pipeline selesai diproses.
    /// 3. **AFTER**: Saat `_next(context)` selesai, eksekusi berlanjut. Catat status response dan durasi.
    /// 
    /// BARIS KODE PENTING:
    /// - `Stopwatch.StartNew()`: Mulai menghitung waktu pemrosesan request secara presisi (nanosecond level).
    /// - `context.Request.Method`: HTTP verb (GET, POST, PUT, DELETE, dll.).
    /// - `context.Request.Path`: URL path endpoint yang dipanggil (misal: /api/products/5).
    /// - `context.Response.StatusCode`: HTTP status code yang dikembalikan oleh controller (200, 400, 404, 500, dll.).
    /// - `stopwatch.ElapsedMilliseconds`: Total waktu pemrosesan dalam milidetik — berguna untuk deteksi endpoint lambat.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        // Mulai pengukur waktu sebelum request diproses
        var stopwatch = Stopwatch.StartNew();
        
        // Log detail request yang masuk (BEFORE)
        _logger.LogInformation("[Middleware LOG] Incoming Request: {Method} {Path}", 
            context.Request.Method, context.Request.Path);

        // Meneruskan request ke middleware berikutnya dalam pipeline
        // Eksekusi kode di bawah baris ini akan menunggu hingga seluruh pipeline selesai
        await _next(context);

        // Hentikan stopwatch setelah response dikembalikan
        stopwatch.Stop();
        
        // Log detail response yang keluar beserta durasi pemrosesan (AFTER)
        _logger.LogInformation("[Middleware LOG] Outgoing Response: {StatusCode} (Processed in {ElapsedMs}ms)", 
            context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
