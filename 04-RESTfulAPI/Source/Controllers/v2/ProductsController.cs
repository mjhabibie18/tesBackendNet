// ============================================================
// Nama File: ProductsController.cs (Version 2)
// Folder: 04-RESTfulAPI/Source/Controllers/v2/
// ============================================================
// 1. PENJELASAN FOLDER (RESTfulAPI/Source/Controllers/v2/):
//    - Tujuan: Mengatur endpoints REST API versi 2.0 yang membawa evolusi skema respon/perubahan fitur.
//    - Kapan Digunakan: Saat klien ingin berinteraksi dengan produk menggunakan spesifikasi standar versi kedua (V2).
//    - Hubungan: Berkomunikasi dengan AppDbContext untuk menulis/membaca data dan menyajikan data dengan manipulasi logika baru.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyajikan endpoints produk versi 2.0 yang mendemonstrasikan breaking changes (harga terdiskon).
//    - Mengapa Diperlukan: Menunjukkan skenario evolusi API di dunia nyata tanpa mengganggu fungsionalitas API V1.
//    - Hubungan File: Memanggil ProductResponseDto.cs dan AppDbContext.cs.
//    - Jika Dihapus: API versi 2.0 tidak tersedia, membatasi fitur baru (diskon harga) bagi klien.
// ============================================================

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.RESTfulAPI.Data;
using TesBackendNet.RESTfulAPI.DTO;
using TesBackendNet.RESTfulAPI.Models;
using TesBackendNet.RESTfulAPI.Common;

namespace TesBackendNet.RESTfulAPI.Controllers.v2;

/// <summary>
/// TUJUAN CLASS:
/// Controller RESTful API yang mengekspos endpoint produk versi 2.0.
/// 
/// ALASAN VERSIONING (API Versioning V2):
/// Kelas ini bertanda `[ApiVersion("2.0")]`. URL akses fisiknya adalah `/api/v2/products`. 
/// Ini memisahkan logika kontroler V2 secara bersih dari kontroler V1 tanpa memicu error ambiguitas rute pada ASP.NET Core engine.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("2.0")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan DbContext untuk interaksi database.
    /// </summary>
    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// FUNGSI METHOD: Mengambil daftar produk V2 dengan diskon harga 10%.
    /// NILAI KEMBALIAN: Task<IActionResult> berisi response envelope sukses beserta data DTO produk V2.
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Query asinkron `ToListAsync` dijalankan untuk mengambil produk dari database tanpa memblokir thread.
    /// 2. Iterasi setiap entitas produk untuk di-mapping menjadi `ProductResponseDto`.
    /// 3. Pada pemetaan harga, dilakukan manipulasi bisnis: `Price = p.Price * 0.9m` (potongan harga 10%).
    /// 4. Mengembalikan HTTP 200 OK dengan payload terbungkus ApiResponse.
    /// 
    /// BARIS KODE PENTING:
    /// - `0.9m`: Huruf `m` di belakang angka desimal memberi tahu compiler C# bahwa literal ini adalah bertipe `decimal`. 
    ///   Jika tidak menyertakan huruf `m`, kompiler akan menganggapnya sebagai `double` dan memicu error kompilasi 
    ///   karena tipe data decimal tidak dapat dikalikan secara implisit dengan double demi menjaga presisi hitungan.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProductsV2()
    {
        var products = await _context.Products.ToListAsync();
        
        // Pemetaan data DTO dengan logika bisnis V2
        var result = products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            // ── Perubahan Fitur di V2: Harga Didiskon 10% ────────────
            Price = p.Price * 0.9m, 
            Stock = p.Stock,
            CreatedAt = p.CreatedAt
        }).ToList();

        // Catatan Desain: Di V2 kita tidak menyertakan tautan HATEOAS untuk mendemonstrasikan
        // fleksibilitas perbedaan struktur response antar versi.
        return Ok(ApiResponse.Success(result, "Berhasil mengambil produk V2 (Diskon 10% Promo)"));
    }
}
