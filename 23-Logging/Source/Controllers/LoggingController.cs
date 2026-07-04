// ============================================================
// LoggingController.cs — Controller Demo Logging
// ============================================================
// Controller ini mendemonstrasikan berbagai level log,
// perbedaan Structured Logging vs String Interpolation,
// serta cara me-log exception dengan stack trace.
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.Logging.Controllers;

[ApiController]
[Route("api/logging")]
public class LoggingController : ControllerBase
{
    private readonly ILogger<LoggingController> _logger;

    public LoggingController(ILogger<LoggingController> logger)
    {
        _logger = logger;
    }

    // ================================================================
    // 1. Demo Berbagai Level Log (Trace s.d Critical)
    // ================================================================
    
    [HttpGet("levels")]
    public IActionResult LogAllLevels()
    {
        _logger.LogTrace("Ini adalah TRACE log (Detail level rendah/development).");
        _logger.LogDebug("Ini adalah DEBUG log (Informasi debugging).");
        _logger.LogInformation("Ini adalah INFORMATION log (Alur normal aplikasi).");
        _logger.LogWarning("Ini adalah WARNING log (Kondisi tidak ideal/janggal tapi bukan error).");
        _logger.LogError("Ini adalah ERROR log (Kesalahan ter-handle).");
        _logger.LogCritical("Ini adalah CRITICAL log (Masalah fatal/system crash).");

        return Ok(new { Message = "Semua level log berhasil dipicu! Silakan cek konsol atau file log." });
    }

    // ================================================================
    // 2. Structured Logging vs String Interpolation
    // ================================================================
    
    [HttpGet("structured")]
    public IActionResult StructuredLoggingDemo([FromQuery] int userId, [FromQuery] string role)
    {
        // ❌ TIDAK DISARANKAN (String Interpolation)
        // Log ini hanya berupa string datar biasa yang sulit di-query di server log (seperti Elastic/Seq)
        _logger.LogInformation($"[Interpolated] User {userId} dengan role {role} masuk sistem pada {DateTime.Now}");

        // ✅ DISARANKAN (Structured Logging)
        // Parameter di-pass sebagai argument terpisah. Log server dapat mem-parsing data
        // ini menjadi fields terpisah: { "UserId": 123, "UserRole": "Admin" }
        _logger.LogInformation("[Structured] User {UserId} dengan role {UserRole} masuk sistem.", userId, role);

        return Ok(new { Message = "Perbandingan log terkirim ke server log." });
    }

    // ================================================================
    // 3. Log Exception dengan Stack Trace
    // ================================================================
    
    [HttpGet("exception")]
    public IActionResult LogExceptionDemo()
    {
        try
        {
            // Simulasi operasi yang menghasilkan exception
            int denominator = 0;
            int result = 100 / denominator;
            return Ok(result);
        }
        catch (DivideByZeroException ex)
        {
            // Kirim objek exception 'ex' sebagai parameter pertama.
            // Serilog akan menyertakan stack trace lengkap secara rapi.
            _logger.LogError(ex, "Terjadi kesalahan fatal saat melakukan perhitungan pembagian.");
            
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Success = false,
                Message = "Perhitungan gagal.",
                ErrorType = ex.GetType().Name
            });
        }
    }
}
