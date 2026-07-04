// ============================================================
// OrmDemoController.cs — Controller Demo EF Core
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.ORM.Data;
using TesBackendNet.ORM.Models;

namespace TesBackendNet.ORM.Controllers;

[ApiController]
[Route("api/orm")]
public class OrmDemoController : ControllerBase
{
    private readonly AppDbContext _context;

    public OrmDemoController(AppDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // 1. Eager Loading Demo (JOIN dengan .Include)
    // ================================================================
    
    [HttpGet("products-eager")]
    public async Task<IActionResult> GetProductsEager()
    {
        // Query database dan gabungkan tabel Categories dalam satu query SQL tunggal (JOIN)
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        return Ok(products);
    }

    // ================================================================
    // 2. Explicit Loading Demo (Muat Navigasi Secara Manual)
    // ================================================================
    
    [HttpGet("product-explicit/{id:int}")]
    public async Task<IActionResult> GetProductExplicit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Produk tidak ditemukan.");

        // Hubungan dengan Category tidak dimuat di awal. Kita muat secara manual:
        await _context.Entry(product)
            .Reference(p => p.Category)
            .LoadAsync();

        return Ok(product);
    }

    // ================================================================
    // 3. AsNoTracking Demo (Query Read-Only Cepat)
    // ================================================================
    
    [HttpGet("products-readonly")]
    public async Task<IActionResult> GetProductsReadOnly()
    {
        // Menggunakan AsNoTracking() untuk performa optimal dan hemat memori
        // karena EF Core tidak merekam perubahan state entity ini di memory-tracker.
        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync();

        return Ok(products);
    }

    // ================================================================
    // 4. Change Tracking & State Transition Demo
    // ================================================================
    
    [HttpPut("update-price/{id:int}")]
    public async Task<IActionResult> UpdateProductPrice(int id, [FromQuery] decimal newPrice)
    {
        // 1. Load data dari DB -> state: Unchanged (Sedang ditracking oleh EF Core)
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // 2. Modifikasi nilai properti -> EF Core otomatis mengubah state menjadi: Modified
        product.Price = newPrice;

        var stateBefore = _context.Entry(product).State.ToString(); // Modified

        // 3. Simpan perubahan ke DB -> EF Core men-generate query UPDATE SQL
        await _context.SaveChangesAsync();

        var stateAfter = _context.Entry(product).State.ToString(); // Unchanged (Kembali netral)

        return Ok(new
        {
            Message = "Harga produk berhasil diperbarui.",
            StateBefore = stateBefore,
            StateAfter = stateAfter,
            Data = product
        });
    }

    // ================================================================
    // 5. Raw SQL Query Demo
    // ================================================================
    
    [HttpGet("raw-sql")]
    public async Task<IActionResult> GetProductsRawSql([FromQuery] decimal minPrice)
    {
        // Menjalankan kueri SQL murni namun tetap aman dari SQL Injection (Interpolated)
        var products = await _context.Products
            .FromSqlInterpolated($"SELECT * FROM Products WHERE Price >= {minPrice}")
            .ToListAsync();

        return Ok(products);
    }
}
