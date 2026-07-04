// ============================================================
// Nama File: ProductsController.cs (Version 1)
// Folder: 04-RESTfulAPI/Source/Controllers/v1/
// ============================================================
// 1. PENJELASAN FOLDER (RESTfulAPI/Source/Controllers/v1/):
//    - Tujuan: Mengatur endpoints REST API versi 1.0 yang didukung oleh dokumentasi hypermedia (HATEOAS).
//    - Kapan Digunakan: Saat klien ingin berinteraksi dengan produk menggunakan spesifikasi standar versi pertama.
//    - Hubungan: Berkomunikasi dengan AppDbContext untuk menulis/membaca data dan menghasilkan representasi tautan navigasi.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan akses API CRUD versi 1.0 dengan integrasi HATEOAS dan Content Negotiation.
//    - Mengapa Diperlukan: Sebagai contoh nyata penerapan maturity level Richardson API tingkat ke-3 (Hypermedia Controls).
//    - Hubungan File: Memanggil ProductResponseDto.cs, LinkDto.cs, dan AppDbContext.cs.
//    - Jika Dihapus: API versi 1.0 tidak lagi tersedia, merusak integrasi dengan aplikasi client yang bergantung pada endpoint v1.
// ============================================================

using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.RESTfulAPI.Data;
using TesBackendNet.RESTfulAPI.DTO;
using TesBackendNet.RESTfulAPI.Models;
using TesBackendNet.RESTfulAPI.Common;

namespace TesBackendNet.RESTfulAPI.Controllers.v1;

/// <summary>
/// TUJUAN CLASS:
/// Controller RESTful API yang mengekspos endpoint produk khusus versi 1.0.
/// 
/// ALASAN VERSIONING (API Versioning):
/// Memungkinkan aplikasi berevolusi tanpa merusak client lama. Di sini kita menggunakan URL Versioning 
/// (yaitu `api/v{version:apiVersion}/products`), sehingga client dapat memilih versi API secara eksplisit di path.
/// 
/// PRINSIP DESIGN:
/// - HATEOAS (Hypermedia As The Engine Of Application State): Menyertakan link navigasi dinamis di dalam 
///   response body sehingga klien tahu aksi apa saja yang valid dilakukan berikutnya (GET, PUT, DELETE).
/// - Content Negotiation: Mengandalkan formatters bawaan agar format output menyesuaikan header HTTP Accept dari klien (JSON atau XML).
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
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
    /// FUNGSI METHOD: Mengambil seluruh daftar produk.
    /// NILAI KEMBALIAN: Task<IActionResult> berisi response envelope sukses beserta data DTO produk yang dilengkapi link HATEOAS.
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Query asinkron `ToListAsync` dijalankan untuk mengambil produk dari database tanpa memblokir thread.
    /// 2. Iterasi setiap entitas produk untuk di-mapping menjadi `ProductResponseDto`.
    /// 3. Untuk setiap DTO, dibuatkan tautan dinamis menggunakan method `CreateLinksForProduct(id)`.
    /// 4. Mengembalikan HTTP 200 OK dengan payload terbungkus ApiResponse.
    /// </summary>
    [HttpGet(Name = nameof(GetProductsV1))]
    public async Task<IActionResult> GetProductsV1()
    {
        // Pengambilan data non-blocking
        var products = await _context.Products.ToListAsync();
        
        // Pemasangan link HATEOAS secara dinamis untuk setiap item produk
        var result = products.Select(p => 
        {
            var dto = MapToDto(p);
            dto.Links = CreateLinksForProduct(p.Id);
            return dto;
        }).ToList();

        return Ok(ApiResponse.Success(result));
    }

    /// <summary>
    /// FUNGSI METHOD: Mengambil detail satu produk berdasarkan ID.
    /// PARAMETER: id (Primary Key produk).
    /// </summary>
    [HttpGet("{id:int}", Name = nameof(GetProductByIdV1))]
    public async Task<IActionResult> GetProductByIdV1(int id)
    {
        // Cari produk berdasarkan Primary Key di memori/database
        var product = await _context.Products.FindAsync(id);
        
        // Skenario jika produk tidak ditemukan (Edge Case)
        if (product == null)
            return NotFound(ApiResponse.Fail($"Produk dengan ID {id} tidak ditemukan."));

        var dto = MapToDto(product);
        // Memasang menu aksi hypermedia yang valid untuk ID spesifik ini
        dto.Links = CreateLinksForProduct(product.Id);

        return Ok(ApiResponse.Success(dto));
    }

    /// <summary>
    /// FUNGSI METHOD: Membuat data produk baru (V1).
    /// PARAMETER: ProductCreateDto.
    /// </summary>
    [HttpPost(Name = nameof(CreateProductV1))]
    public async Task<IActionResult> CreateProductV1([FromBody] ProductCreateDto dto)
    {
        // Mengubah payload request menjadi model entitas database
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock
        };

        // Menambahkan entitas ke tracking context EF Core
        _context.Products.Add(product);
        
        // Menyimpan perubahan fisik ke SQL Server
        await _context.SaveChangesAsync();

        var responseDto = MapToDto(product);
        responseDto.Links = CreateLinksForProduct(product.Id);

        // CreatedAtRoute mengembalikan status 201 dengan header Location yang menunjuk ke URL detail produk yang baru dibuat
        return CreatedAtRoute(
            nameof(GetProductByIdV1), 
            new { version = "1.0", id = product.Id }, 
            ApiResponse.Success(responseDto, "Produk berhasil dibuat."));
    }

    /// <summary>
    /// FUNGSI METHOD: Memetakan entitas produk ke objek transfer data (DTO).
    /// </summary>
    private static ProductResponseDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        CreatedAt = p.CreatedAt
    };

    /// <summary>
    /// FUNGSI METHOD: Menghasilkan kumpulan tautan hypermedia dinamis (HATEOAS).
    /// PARAMETER: id (ID produk terkait).
    /// 
    /// ALASAN IMPLEMENTASI:
    /// - Url.Link: Menggunakan route name (seperti GetProductByIdV1) untuk merender URL absolut lengkap secara otomatis 
    ///   berdasarkan domain/port server saat ini.
    /// - Menghindari hardcode URL mentah, sehingga jika routing controller berubah, generator URL ini otomatis menyesuaikan.
    /// </summary>
    private List<LinkDto> CreateLinksForProduct(int id)
    {
        var links = new List<LinkDto>();

        // Tautan ke diri sendiri (Self-reference)
        var selfUrl = Url.Link(nameof(GetProductByIdV1), new { id }) ?? "";
        links.Add(new LinkDto(selfUrl, "self", "GET"));

        // Tautan aksi memperbarui produk ini
        links.Add(new LinkDto(selfUrl, "update_product", "PUT"));

        // Tautan aksi menghapus produk ini
        links.Add(new LinkDto(selfUrl, "delete_product", "DELETE"));

        return links;
    }
}
