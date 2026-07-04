// ============================================================
// AdminController.cs — Demonstrasi Role-Based Access Control
// ============================================================
// Controller ini mendemonstrasikan bagaimana membatasi akses
// menggunakan kombinasi Authentication (Siapa Anda?) dan
// Authorization (Bolehkah Anda mengakses ini?).
// ============================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.Authorization.Controllers;

/// <summary>
/// Controller ini diproteksi secara keseluruhan.
/// Hanya user yang sudah login (punya JWT) dan memiliki Role "Admin" atau "Manager"
/// yang bisa mengakses endpoints di dalamnya (berlaku sebagai default proteksi tingkat class).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Manager")] 
public class AdminController : ControllerBase
{
    // ================================================================
    // GET /api/admin/dashboard
    // ================================================================
    
    /// <summary>
    /// Karena tidak ada atribut khusus di method ini, maka ia mewarisi
    /// atribut dari tingkat class, yaitu: [Authorize(Roles = "Admin,Manager")]
    /// 
    /// Artinya: Admin DAN Manager bisa mengakses endpoint ini.
    /// User biasa akan mendapat HTTP 403 Forbidden.
    /// </summary>
    [HttpGet("dashboard")]
    public IActionResult GetDashboard()
    {
        return Ok(new 
        { 
            message = "Selamat datang di Dashboard Admin/Manager!",
            accessedBy = User.Identity?.Name ?? "Unknown" 
        });
    }

    // ================================================================
    // GET /api/admin/reports
    // ================================================================

    /// <summary>
    /// Method ini MENIMPA (override) pengaturan dari class.
    /// Endpoint ini SANGAT STRICT: HANYA role "Admin" yang boleh akses.
    /// 
    /// Meskipun Manager bisa mengakses controller ini secara umum,
    /// jika Manager mencoba hit endpoint ini, akan di-block (403 Forbidden).
    /// </summary>
    [HttpGet("reports")]
    [Authorize(Roles = "Admin")] // HANYA Admin
    public IActionResult GetFinancialReports()
    {
        return Ok(new 
        { 
            message = "Laporan Keuangan Perusahaan (SANGAT RAHASIA)",
            data = new[] { "January: $5000", "February: $7000" }
        });
    }

    // ================================================================
    // GET /api/admin/public-info
    // ================================================================

    /// <summary>
    /// Atribut [AllowAnonymous] menonaktifkan SEMUA proteksi otorisasi
    /// khusus untuk endpoint ini. Siapapun (bahkan yang belum login)
    /// bisa mengakses endpoint ini.
    /// </summary>
    [HttpGet("public-info")]
    [AllowAnonymous]
    public IActionResult GetPublicInfo()
    {
        return Ok(new 
        { 
            message = "Informasi ini terbuka untuk umum tanpa perlu login." 
        });
    }

    // ================================================================
    // Penjelasan Kode Status HTTP dalam Otorisasi:
    // ================================================================
    // HTTP 401 (Unauthorized):
    // - Token tidak ada (belum login).
    // - Token salah format atau expired.
    // - (Solusi: Arahkan user ke halaman Login)
    //
    // HTTP 403 (Forbidden):
    // - Token VALID (sudah login), TAPI..
    // - Role/Permission tidak mencukupi untuk endpoint tersebut.
    // - (Solusi: Tampilkan pesan "Anda tidak memiliki akses ke halaman ini")
}
