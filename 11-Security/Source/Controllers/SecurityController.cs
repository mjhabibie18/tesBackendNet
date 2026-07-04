// ============================================================
// SecurityController.cs — Controller Demo Security
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Web;
using TesBackendNet.Security.Data;

namespace TesBackendNet.Security.Controllers;

[ApiController]
[Route("api/security")]
public class SecurityController : ControllerBase
{
    private readonly AppDbContext _context;

    public SecurityController(AppDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // 1. SQL Injection Demo & Prevention
    // ================================================================

    /// <summary>
    /// Contoh parameterization query yang AMAN.
    /// EF Core otomatis menggunakan SQL parameterization untuk LINQ.
    /// </summary>
    [HttpGet("sql-safe")]
    public async Task<IActionResult> SearchSafe([FromQuery] string name)
    {
        // LINQ Query: AMAN dari SQL Injection
        var products = await _context.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync();

        return Ok(new { Method = "Safe (LINQ)", Data = products });
    }

    /// <summary>
    /// Contoh raw SQL query yang AMAN menggunakan FromSqlInterpolated.
    /// Interpolasi string diubah menjadi DB Parameter oleh EF Core.
    /// </summary>
    [HttpGet("sql-raw-safe")]
    public async Task<IActionResult> SearchRawSafe([FromQuery] string name)
    {
        // FromSqlInterpolated: AMAN karena parameter dibungkus SqlCommand parameters
        var products = await _context.Products
            .FromSqlInterpolated($"SELECT * FROM Products WHERE Name LIKE {"%" + name + "%"}")
            .ToListAsync();

        return Ok(new { Method = "Safe (FromSqlInterpolated)", Data = products });
    }

    // ================================================================
    // 2. XSS (Cross-Site Scripting) Sanitization
    // ================================================================

    /// <summary>
    /// Menyediakan fungsi encode input HTML untuk mencegah XSS.
    /// </summary>
    [HttpPost("xss-sanitize")]
    public IActionResult SanitizeInput([FromBody] UserCommentRequest request)
    {
        // Mengubah tag script berbahaya menjadi text biasa (HTML Encoded)
        // Misal: <script>alert('hack')</script> -> &lt;script&gt;alert('hack')&lt;/script&gt;
        var sanitizedComment = HttpUtility.HtmlEncode(request.Comment);

        return Ok(new
        {
            Original = request.Comment,
            Sanitized = sanitizedComment,
            Message = "Input berhasil disanitasi!"
        });
    }

    // ================================================================
    // 3. Rate Limiting Demo (.NET 8 Built-in Rate Limiter)
    // ================================================================

    /// <summary>
    /// Endpoint yang dilindungi oleh policy "FixedWindow" Rate Limiting.
    /// </summary>
    [HttpGet("rate-limited")]
    [EnableRateLimiting("FixedWindow")]
    public IActionResult GetRateLimited()
    {
        return Ok(new { Message = "Berhasil mengakses endpoint yang dibatasi rate limit!" });
    }
}

public class UserCommentRequest
{
    public string Comment { get; set; } = string.Empty;
}
