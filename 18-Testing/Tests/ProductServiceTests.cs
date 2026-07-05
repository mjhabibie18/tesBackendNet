// ============================================================
// Nama File: ProductServiceTests.cs — Unit Tests xUnit + Moq + FluentAssertions
// Folder: 18-Testing/Tests/
// ============================================================
// 1. PENJELASAN FOLDER (Testing/Tests):
//    - Tujuan: Menyimpan seluruh kode pengujian otomatis untuk modul Testing.
//    - Kapan Digunakan: Dijalankan secara otomatis oleh test runner (dotnet test) atau CI pipeline.
//    - Hubungan: Menguji ProductService.cs dengan cara menggantikan IProductRepository dengan Mock object (tidak menyentuh database nyata).
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan 3 test case yang mencakup skenario sukses, gagal, dan edge case.
//    - Mengapa Diperlukan: Unit test memberikan konfirmasi bahwa logika bisnis berjalan benar sebelum kode di-deploy ke produksi.
//    - Jika Dihapus: Tidak ada jaminan otomatis bahwa logika Service berjalan benar ketika ada perubahan kode.
//
// 3. LIBRARY YANG DIGUNAKAN:
//    - xUnit: Framework testing utama (.NET standard).
//    - Moq: Library untuk membuat mock object dari interface.
//    - FluentAssertions: Library assertion yang membuat kode test lebih ekspresif dan mudah dibaca.
// ============================================================

using FluentAssertions;
using Moq;
using TesBackendNet.TestingDemo.DTOs;
using TesBackendNet.TestingDemo.Models;
using TesBackendNet.TestingDemo.Repositories;
using TesBackendNet.TestingDemo.Services;
using Xunit;

namespace TesBackendNet.TestingDemo.Tests;

/// <summary>
/// TUJUAN CLASS:
/// Kelas yang berisi koleksi unit test untuk ProductService menggunakan pola AAA (Arrange-Act-Assert).
/// 
/// POLA AAA (ARRANGE-ACT-ASSERT):
/// - Arrange: Siapkan data, mock, dan kondisi awal sebelum test.
/// - Act: Eksekusi method yang ingin diuji.
/// - Assert: Verifikasi bahwa hasil eksekusi sesuai ekspektasi.
/// 
/// MENGAPA MENGGUNAKAN MOCK (Moq)?
/// Mock menggantikan dependensi nyata (seperti database) dengan "pura-pura" yang dapat dikontrol:
/// - Tidak perlu koneksi database nyata → test lebih cepat.
/// - Dapat mensimulasikan kondisi apapun (data ditemukan, tidak ditemukan, error, dll).
/// - Test berjalan secara terisolasi tanpa side effect pada data produksi.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    /// <summary>
    /// CONSTRUCTOR:
    /// xUnit memanggil constructor ini SEBELUM setiap test method dijalankan.
    /// Ini memastikan setiap test memulai dengan state yang bersih (fresh mock object), tanpa state dari test sebelumnya.
    /// </summary>
    public ProductServiceTests()
    {
        // Membuat mock object dari interface IProductRepository
        _mockRepo = new Mock<IProductRepository>();
        
        // Menyuntikkan mock ke ProductService — menggantikan implementasi repository nyata
        _service = new ProductService(_mockRepo.Object);
    }

    // ================================================================
    // TEST CASE 1: Validasi exception saat nama produk duplikat
    // ================================================================
    
    /// <summary>
    /// TEST: CreateAsync harus melempar InvalidOperationException jika nama sudah ada.
    /// 
    /// CARA MEMBACA NAMA TEST: CreateAsync_WhenNameExists_ThrowsException
    ///   Format: [MethodName]_[Kondisi]_[HasilYangDiharapkan]
    ///   Ini adalah konvensi penamaan test yang sangat direkomendasikan.
    /// 
    /// [Fact]: Atribut xUnit yang menandai method sebagai satu test case tunggal (tanpa parameter).
    /// 
    /// PENJELASAN _mockRepo.Setup:
    ///   `_mockRepo.Setup(r => r.ExistsByNameAsync(dto.Name)).ReturnsAsync(true)`
    ///   Artinya: "Ketika ExistsByNameAsync dipanggil dengan dto.Name, balasan seolah-olah mengembalikan true."
    ///   Ini mensimulasikan kondisi nama produk sudah ada di database.
    /// </summary>
    [Fact]
    public async Task CreateAsync_WhenNameExists_ThrowsException()
    {
        // ── Arrange ────────────────────────────────────────────────
        var dto = new ProductCreateDto { Name = "Laptop Gaming", Price = 15000000, Stock = 5 };
        
        // Instruksikan mock untuk mengembalikan true (nama sudah ada)
        _mockRepo.Setup(r => r.ExistsByNameAsync(dto.Name)).ReturnsAsync(true);

        // ── Act ─────────────────────────────────────────────────────
        // Simpan delegate ke function yang akan melempar exception (bukan langsung await)
        var act = async () => await _service.CreateAsync(dto);

        // ── Assert ──────────────────────────────────────────────────
        // FluentAssertions: Memverifikasi bahwa act melempar exception tipe yang benar dengan pesan yang cocok
        // `.WithMessage("*sudah ada*")` menggunakan wildcard (*) untuk pencocokan substring
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*sudah ada*");
    }

    // ================================================================
    // TEST CASE 2: Validasi mapping DTO saat data ditemukan (Parameterized Test)
    // ================================================================
    
    /// <summary>
    /// TEST: GetByIdAsync harus mengembalikan DTO yang benar saat produk ditemukan.
    /// 
    /// [Theory] + [InlineData]: Menjalankan test yang sama berkali-kali dengan data berbeda.
    /// Setiap baris [InlineData] menghasilkan satu test run yang independen.
    /// Ini lebih efisien daripada membuat metode [Fact] terpisah untuk setiap skenario data.
    /// </summary>
    [Theory]
    [InlineData(1, "Laptop", 10000000, 5)]
    [InlineData(2, "Mouse Wireless", 150000, 20)]
    public async Task GetByIdAsync_WhenExists_ReturnsDto(
        int id, string name, decimal price, int stock)
    {
        // ── Arrange ────────────────────────────────────────────────
        var product = new Product { Id = id, Name = name, Price = price, Stock = stock };
        
        // Mock: Ketika GetByIdAsync(id) dipanggil, kembalikan product yang sudah disiapkan
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        // ── Act ─────────────────────────────────────────────────────
        var result = await _service.GetByIdAsync(id);

        // ── Assert ──────────────────────────────────────────────────
        result.Should().NotBeNull();                // DTO tidak boleh null
        result!.Id.Should().Be(id);                // ID harus cocok
        result.Name.Should().Be(name);             // Nama harus cocok (membuktikan mapping benar)
        result.Price.Should().Be(price);           // Harga harus cocok
        result.Stock.Should().Be(stock);           // Stok harus cocok
    }

    // ================================================================
    // TEST CASE 3: Validasi nilai null saat data tidak ditemukan
    // ================================================================
    
    /// <summary>
    /// TEST: GetByIdAsync harus mengembalikan null jika produk tidak ditemukan.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // ── Arrange ────────────────────────────────────────────────
        // Mock: ID 999 tidak ada di "database" — kembalikan null
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        // ── Act ─────────────────────────────────────────────────────
        var result = await _service.GetByIdAsync(999);

        // ── Assert ──────────────────────────────────────────────────
        result.Should().BeNull(); // Service harus meneruskan null dari repository tanpa modifikasi
    }
}
