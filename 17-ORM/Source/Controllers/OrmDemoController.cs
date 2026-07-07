// ============================================================
// Nama File: OrmDemoController.cs — Controller Demo Entity Framework Core (ORM)
// Folder: 17-ORM/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (ORM):
//    - Tujuan: Mendemonstrasikan berbagai teknik pengambilan data menggunakan EF Core sebagai ORM (Object-Relational Mapper).
//    - Kapan Digunakan: Saat membutuhkan kontrol lebih detail atas cara EF Core memuat data relasional dan mengelola memori.
//    - Hubungan: Controller ini menggunakan AppDbContext untuk mengeksekusi kueri dengan berbagai strategi loading.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengekspos 5 endpoint yang masing-masing mendemonstrasikan pola EF Core berbeda:
//      Eager Loading, Explicit Loading, AsNoTracking, Change Tracking, dan Raw SQL Query.
//    - Mengapa Diperlukan: Memahami perbedaan strategi loading adalah kunci optimasi performa aplikasi backend berbasis EF Core.
//    - Jika Dihapus: Tidak ada demonstrasi teknik ORM yang fungsional pada modul ini.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.ORM.Data;
using TesBackendNet.ORM.Models;

namespace TesBackendNet.ORM.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang menampilkan 5 teknik berbeda dalam menggunakan Entity Framework Core sebagai ORM.
/// 
/// APA ITU ORM (Object-Relational Mapper)?
/// ORM adalah layer yang menerjemahkan antara dunia objek C# dan dunia tabel relasional SQL Server.
/// Daripada menulis: `SELECT * FROM Products WHERE Id = 1`
/// Kita cukup menulis: `_context.Products.FindAsync(1)`
/// EF Core yang menerjemahkannya ke SQL secara otomatis.
/// </summary>
[ApiController]
[Route("api/orm")]
public class OrmDemoController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan AppDbContext dari DI Container.
    /// </summary>
    public OrmDemoController(AppDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // 1. EAGER LOADING (Include — JOIN dalam satu query SQL)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mengambil produk beserta data Category-nya dalam satu query SQL (JOIN).
    /// 
    /// EAGER LOADING (.Include()):
    /// EF Core menghasilkan SQL dengan JOIN sehingga produk dan kategorinya dimuat dalam satu round-trip ke database.
    /// 
    /// SQL yang dihasilkan (kira-kira):
    ///   SELECT p.*, c.* FROM Products p
    ///   LEFT JOIN Categories c ON p.CategoryId = c.Id
    /// 
    /// KAPAN GUNAKAN EAGER LOADING:
    /// Saat data relasional pasti akan digunakan bersama data utama (misal: tampilkan produk dengan nama kategorinya).
    /// </summary>
    [HttpGet("products-eager")]
    public async Task<IActionResult> GetProductsEager()
    {
        // .Include(p => p.Category): Instruksi eager loading — gabungkan tabel Categories
        var products = await _context.Products
            .Include(p => p.Category)  // SQL LEFT JOIN ke tabel Categories
            .ToListAsync();

        return Ok(products);
    }

    // ================================================================
    // 2. EXPLICIT LOADING (Muat Navigasi Secara Manual Setelah Query)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mengambil produk, kemudian secara eksplisit memuat data Category secara terpisah.
    /// 
    /// EXPLICIT LOADING (Entry().Reference().LoadAsync()):
    /// - Produk diambil dulu → 1 query ke database.
    /// - Category dimuat kemudian secara manual → 1 query terpisah ke database.
    /// - Total: 2 round-trip ke database.
    /// 
    /// KAPAN GUNAKAN EXPLICIT LOADING:
    /// Saat data relasional hanya dibutuhkan secara kondisional (misal: hanya muat kategori jika pengguna klik "detail").
    /// Ini lebih efisien dibandingkan always-eager-loading jika data relasional sering tidak diperlukan.
    /// 
    /// BARIS KODE PENTING:
    /// `_context.Entry(product).Reference(p => p.Category).LoadAsync()`:
    /// - `.Entry(product)`: Mendapatkan EntityEntry dari tracker EF Core untuk objek product ini.
    /// - `.Reference(p => p.Category)`: Menentukan navigation property tunggal (bukan collection) yang akan dimuat.
    /// - `.LoadAsync()`: Mengeksekusi query SQL terpisah ke database untuk mengambil Category.
    /// </summary>
    [HttpGet("product-explicit/{id:int}")]
    public async Task<IActionResult> GetProductExplicit(int id)
    {
        // Query pertama: Ambil produk saja (tanpa Category)
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Produk tidak ditemukan.");

        // Query kedua (Explicit Loading): Muat data Category secara manual
        await _context.Entry(product)
            .Reference(p => p.Category)  // Tentukan property relasi yang akan dimuat
            .LoadAsync();                  // Eksekusi query SQL terpisah

        return Ok(product);
    }

    // ================================================================
    // 3. AsNoTracking (Query Read-Only Optimal)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mengambil produk tanpa Change Tracking untuk performa optimal.
    /// 
    /// AsNoTracking() vs Query Biasa:
    /// - Query biasa: EF Core menyimpan snapshot setiap entitas di Identity Map (Memory) untuk mendeteksi perubahan.
    ///   Ini memerlukan alokasi memori dan CPU overhead lebih besar.
    /// - AsNoTracking(): EF Core TIDAK menyimpan snapshot. Data langsung dibuang setelah dikirim ke klien.
    ///   Lebih cepat ~30% dan menggunakan lebih sedikit memori.
    /// 
    /// KAPAN GUNAKAN AsNoTracking:
    /// Selalu gunakan untuk query GET (read-only) yang tidak akan diikuti oleh operasi UPDATE/DELETE.
    /// Jangan gunakan jika Anda akan memanggil SaveChangesAsync() untuk menyimpan perubahan entity ini.
    /// </summary>
    [HttpGet("products-readonly")]
    public async Task<IActionResult> GetProductsReadOnly()
    {
        // AsNoTracking(): EF Core tidak merekam state entity → lebih cepat & hemat memori
        var products = await _context.Products
            .AsNoTracking()
            .ToListAsync();

        return Ok(products);
    }

    // ================================================================
    // 4. CHANGE TRACKING & STATE TRANSITION
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mendemonstrasikan lifecycle state entity di EF Core Change Tracker.
    /// PARAMETER: id (ID produk), newPrice (harga baru).
    /// 
    /// STATE ENTITY DI EF CORE (EntityState):
    /// - Detached: Entity tidak diketahui oleh tracker (belum di-query atau sudah di-dispose).
    /// - Unchanged: Entity di-query tapi tidak ada perubahan (state awal setelah FindAsync).
    /// - Modified: Satu atau lebih properti entity telah diubah.
    /// - Added: Entity baru ditambahkan dan akan di-INSERT saat SaveChanges.
    /// - Deleted: Entity ditandai untuk di-DELETE saat SaveChanges.
    /// 
    /// ALUR EKSEKUSI:
    /// 1. FindAsync → State: Unchanged.
    /// 2. product.Price = newPrice → EF Core mendeteksi perubahan → State: Modified.
    /// 3. SaveChangesAsync → EF Core mengenerate SQL UPDATE hanya untuk kolom yang berubah → State: Unchanged.
    /// </summary>
    [HttpPut("update-price/{id:int}")]
    public async Task<IActionResult> UpdateProductPrice(int id, [FromQuery] decimal newPrice)
    {
        // State: Detached → setelah FindAsync → State: Unchanged
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        // Modifikasi properti → EF Core Change Tracker otomatis mengubah state ke: Modified
        product.Price = newPrice;
        var stateBefore = _context.Entry(product).State.ToString(); // "Modified"

        // SaveChangesAsync: EF Core generate SQL UPDATE hanya untuk kolom Price yang berubah
        await _context.SaveChangesAsync();
        var stateAfter = _context.Entry(product).State.ToString();  // "Unchanged" (kembali netral)

        return Ok(new
        {
            Message     = "Harga produk berhasil diperbarui.",
            StateBefore = stateBefore,
            StateAfter  = stateAfter,
            Data        = product
        });
    }

    // ================================================================
    // 5. RAW SQL QUERY (FromSqlInterpolated)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Menjalankan kueri SQL murni dengan aman menggunakan FromSqlInterpolated.
    /// PARAMETER: minPrice (batas harga minimum).
    /// 
    /// MENGAPA FromSqlInterpolated AMAN:
    /// Meskipun terlihat seperti string interpolation biasa, EF Core mengintersepsi ekspresi `$"...{minPrice}..."`
    /// dan mengubah `{minPrice}` menjadi SQL parameter (@p0) secara otomatis.
    /// Penyerang TIDAK dapat menyuntikkan SQL berbahaya melalui parameter ini.
    /// 
    /// KAPAN GUNAKAN RAW SQL:
    /// - Saat query sangat kompleks dan LINQ tidak efisien (query dengan nested subquery kompleks).
    /// - Saat menggunakan fitur database spesifik yang belum didukung LINQ EF Core (misal: FULL-TEXT SEARCH).
    /// </summary>
    [HttpGet("raw-sql")]
    public async Task<IActionResult> GetProductsRawSql([FromQuery] decimal minPrice)
    {
        // FromSqlInterpolated: SQL mentah namun parameterized (AMAN dari SQL Injection)
        var products = await _context.Products
            .FromSqlInterpolated($"SELECT * FROM Products WHERE Price >= {minPrice}")
            .ToListAsync();

        return Ok(products);
    }
}
