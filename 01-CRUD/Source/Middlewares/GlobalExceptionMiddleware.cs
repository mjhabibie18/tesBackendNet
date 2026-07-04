// ============================================================
// GlobalExceptionMiddleware.cs — Global Error Handler
// ============================================================
// Middleware ini menangkap SEMUA exception yang tidak ter-handle
// di controller/service/repository dan mengkonversi menjadi
// response JSON yang konsisten.
//
// Mengapa perlu Global Exception Handler?
//   - Tanpa ini: exception yang tidak ter-handle menyebabkan
//     server mengembalikan HTML error page (bukan JSON)
//   - Dengan ini: semua error selalu dalam format JSON yang konsisten
//   - DRY: tidak perlu try-catch di setiap controller
//
// Middleware Pipeline Position:
//   - Harus didaftarkan PERTAMA di pipeline
//   - Agar bisa catch exception dari semua middleware setelahnya
// ============================================================

using System.Text.Json;
using TesBackendNet.CRUD.Common;

namespace TesBackendNet.CRUD.Middlewares;

/// <summary>
/// Global exception handler middleware.
/// Mengkonversi semua exception tidak tertangkap menjadi JSON response.
/// </summary>
public class GlobalExceptionMiddleware
{
    // ── Private Fields ────────────────────────────────────────
    // _next: delegate ke middleware berikutnya dalam pipeline
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    // ── InvokeAsync ───────────────────────────────────────────
    // Method ini dipanggil oleh ASP.NET Core pipeline untuk setiap request.
    // Pola try-catch di sini menangkap semua exception dari downstream middleware.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Teruskan request ke middleware/controller berikutnya
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log exception untuk debugging
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            // Konversi exception ke JSON response
            await HandleExceptionAsync(context, ex);
        }
    }

    // ── HandleException ───────────────────────────────────────
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set response type ke JSON
        context.Response.ContentType = "application/json";

        // Tentukan HTTP status code berdasarkan jenis exception
        // Ini adalah mapping exception → status code
        var (statusCode, message) = exception switch
        {
            // 400 Bad Request
            ArgumentException         => (StatusCodes.Status400BadRequest,   exception.Message),
            InvalidOperationException => (StatusCodes.Status409Conflict,     exception.Message), // 409 Conflict
            // 404 Not Found
            KeyNotFoundException      => (StatusCodes.Status404NotFound,     exception.Message),
            // 401 Unauthorized
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Akses ditolak"),
            // 500 untuk semua error yang tidak dikenal
            _ => (StatusCodes.Status500InternalServerError, "Terjadi kesalahan pada server. Silakan coba lagi.")
        };

        context.Response.StatusCode = statusCode;

        // Buat response JSON
        var response = ApiResponse.Fail(message);

        // Serialize ke JSON
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
