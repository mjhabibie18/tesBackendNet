// ============================================================
// Nama File: ProductService.cs — Business Logic untuk Unit Testing Demo
// Folder: 18-Testing/Source/Services/
// ============================================================
// 1. PENJELASAN FOLDER (Testing):
//    - Tujuan: Mendemonstrasikan cara menulis unit test yang baik untuk Service Layer menggunakan xUnit, Moq, dan FluentAssertions.
//    - Kapan Digunakan: Saat ingin memverifikasi bahwa logika bisnis berfungsi benar tanpa menghubungkan ke database nyata.
//    - Hubungan: ProductService bergantung pada IProductRepository (interface) yang dapat di-mock untuk isolasi pengujian.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengimplementasikan logika bisnis untuk demo unit testing yang dapat diuji sepenuhnya.
//    - Mengapa Diperlukan: Service yang bergantung pada interface (bukan implementasi konkrit) dapat di-mock dan diuji tanpa database.
//    - Jika Dihapus: Tidak ada logika bisnis yang dapat diuji di modul ini.
// ============================================================

using TesBackendNet.TestingDemo.DTOs;
using TesBackendNet.TestingDemo.Models;
using TesBackendNet.TestingDemo.Repositories;

namespace TesBackendNet.TestingDemo.Services;

/// <summary>
/// TUJUAN CLASS:
/// Service Layer yang mengimplementasikan logika bisnis produk, dirancang untuk dapat diuji secara terisolasi.
/// 
/// MENGAPA SERVICE INI MUDAH DIUJI (TESTABLE)?
/// - Menggunakan Constructor Injection dengan interface IProductRepository.
/// - Unit test dapat menyuntikkan Mock<IProductRepository> menggantikan repository nyata.
/// - Tidak ada `new ProductRepository()` di dalam class ini (tidak ada tight coupling).
/// </summary>
public class ProductService
{
    private readonly IProductRepository _repo;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan repository melalui interface.
    /// </summary>
    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// FUNGSI METHOD: Mencari satu produk berdasarkan ID.
    /// PARAMETER: id (Primary Key produk).
    /// NILAI KEMBALIAN: Task<ProductResponseDto?> — nullable DTO (null jika tidak ditemukan).
    /// 
    /// TESTCASE YANG DIUJI:
    /// - Skenario Sukses: ID ditemukan → mengembalikan DTO yang terisi dengan data produk.
    /// - Skenario Gagal: ID tidak ditemukan → mengembalikan null.
    /// </summary>
    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        
        // Jika tidak ditemukan, kembalikan null (controller yang akan menghasilkan HTTP 404)
        if (product == null) return null;

        // Mapping: Product entity → ProductResponseDto
        return new ProductResponseDto
        {
            Id    = product.Id,
            Name  = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }

    /// <summary>
    /// FUNGSI METHOD: Membuat produk baru.
    /// PARAMETER: ProductCreateDto (data input dari klien).
    /// NILAI KEMBALIAN: Task<ProductResponseDto> — DTO produk yang baru berhasil dibuat.
    /// EXCEPTION: InvalidOperationException jika nama produk sudah ada (validasi unik).
    /// 
    /// TESTCASE YANG DIUJI:
    /// - Skenario Sukses: Nama belum ada → produk berhasil dibuat.
    /// - Skenario Gagal: Nama sudah ada → melempar InvalidOperationException.
    /// </summary>
    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        // Validasi keunikan nama produk (aturan bisnis)
        if (await _repo.ExistsByNameAsync(dto.Name))
        {
            throw new InvalidOperationException($"Produk dengan nama '{dto.Name}' sudah ada.");
        }

        var product = new Product
        {
            Name  = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };

        await _repo.AddAsync(product);

        return new ProductResponseDto
        {
            Id    = product.Id,
            Name  = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}
