// ============================================================
// Nama File: DesignDemoController.cs — Controller Demo Database Design
// Folder: 06-DatabaseDesign/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (DatabaseDesign):
//    - Tujuan: Mendemonstrasikan penerapan konsep desain database relasional secara langsung melalui EF Core:
//      Primary Key, Foreign Key, Unique Constraint, Check Constraint, Index, dan Normalisasi.
//    - Kapan Digunakan: Saat membangun skema database baru atau me-review desain tabel yang sudah ada.
//    - Hubungan: Controller ini memanggil AppDbContext yang menerapkan konfigurasi entitas melalui IEntityTypeConfiguration.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengekspos endpoint untuk membaca dan menulis data yang memverifikasi constraint database.
//    - Mengapa Diperlukan: Membuktikan secara fungsional bahwa constraint (UNIQUE, CHECK) benar-benar diberlakukan oleh database.
//    - Jika Dihapus: Tidak ada endpoint untuk menguji desain database secara fungsional.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.DatabaseDesign.Data;
using TesBackendNet.DatabaseDesign.Models;

namespace TesBackendNet.DatabaseDesign.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang memverifikasi penerapan constraint database melalui operasi baca dan tulis.
/// </summary>
[ApiController]
[Route("api/design")]
public class DesignDemoController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan AppDbContext dari DI Container.
    /// </summary>
    public DesignDemoController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// FUNGSI METHOD: Mengambil semua User beserta daftar Order masing-masing (JOIN One-to-Many).
    /// NILAI KEMBALIAN: HTTP 200 OK berisi List User + Orders.
    /// 
    /// `.Include(u => u.Orders)`: SQL LEFT JOIN ke tabel Orders.
    /// Tanpa Include, properti Orders akan berisi collection kosong (lazy loading tidak aktif).
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.Include(u => u.Orders).ToListAsync();
        return Ok(users);
    }

    /// <summary>
    /// FUNGSI METHOD: Membuat User baru dan memverifikasi penerapan UNIQUE & CHECK constraint.
    /// PARAMETER: UserDto (username, email, age).
    /// NILAI KEMBALIAN: HTTP 201 Created jika berhasil, atau HTTP 400 jika constraint dilanggar.
    /// 
    /// SKENARIO YANG DAPAT DIUJI:
    /// 1. Username atau Email yang sudah ada → Database melempar UNIQUE constraint violation.
    /// 2. Age negatif (misal: -5) → Database melempar CHECK constraint violation.
    /// 
    /// ALASAN MENANGKAP DbUpdateException:
    /// `DbUpdateException` adalah tipe exception yang dilempar EF Core saat database menolak operasi INSERT/UPDATE.
    /// `.InnerException?.Message` berisi pesan error detail dari SQL Server tentang constraint mana yang dilanggar.
    /// </summary>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
    {
        var newUser = new User
        {
            Username  = dto.Username,
            Email     = dto.Email,
            Age       = dto.Age,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync(); // Jika constraint dilanggar, exception dilempar di sini
            return Created("", newUser);
        }
        catch (DbUpdateException ex)
        {
            // Menangkap error constraint pelanggaran UNIQUE atau CHECK dari database SQL Server
            return BadRequest(new
            {
                Success = false,
                Message = "Gagal membuat User. Terjadi pelanggaran Constraint database (UNIQUE / CHECK / NOT NULL).",
                Details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }
}

/// <summary>
/// TUJUAN CLASS:
/// DTO sederhana untuk menerima data User baru dari request body.
/// Hanya berisi field yang boleh diinput oleh klien (tidak termasuk Id, IsActive, CreatedAt yang diset server).
/// </summary>
public class UserDto
{
    /// <summary>Nama pengguna yang unik di sistem.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Alamat email yang unik di sistem.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Usia pengguna (harus antara 0 dan 150 — dijaga oleh CHECK constraint database).</summary>
    public int Age { get; set; }
}
