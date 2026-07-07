// ============================================================
// Nama File: DemoResponseController.cs — Controller Demonstrasi Standar Respon API
// Folder: 09-ApiResponse/Source/Controllers/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menampilkan 3 skenario respons API: single resource, paginated list, dan error.
//    - Mengapa Diperlukan: Membuktikan bahwa menggunakan ApiResponse<T> sebagai wrapper menghasilkan format JSON
//      yang konsisten sehingga klien hanya perlu satu logika parsing untuk semua jenis respons.
//    - Hubungan File: Menggunakan ApiResponse.cs dan PagedData<T> dari folder Common.
//    - Jika Dihapus: Tidak ada demonstrasi fungsional dari standar response wrapper.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TesBackendNet.ApiResponse.Common;

namespace TesBackendNet.ApiResponse.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang menampilkan pola penggunaan ApiResponse<T> di berbagai skenario respons REST API.
/// </summary>
[ApiController]
[Route("api/responses")]
public class DemoResponseController : ControllerBase
{
    /// <summary>
    /// FUNGSI METHOD: Mengambil Trace ID unik dari konteks HTTP request saat ini.
    /// 
    /// ALASAN MENGGUNAKAN Activity.Current?.Id:
    /// - `Activity.Current?.Id` adalah Trace ID dari .NET Distributed Tracing (W3C TraceContext standard).
    ///   Ini berguna saat mengintegrasikan dengan sistem APM seperti Jaeger, Zipkin, atau Azure App Insights.
    /// - `HttpContext.TraceIdentifier` adalah fallback: ID unik yang dibuat per request oleh ASP.NET Core.
    ///   Formatnya berbeda dan tidak kompatibel dengan standar W3C.
    /// - Operator `??` memilih alternatif kanan jika kiri bernilai null.
    /// </summary>
    private string GetTraceId() => Activity.Current?.Id ?? HttpContext.TraceIdentifier;

    /// <summary>
    /// FUNGSI METHOD: Mengembalikan satu objek produk dalam format ApiResponse terbungkus.
    /// NILAI KEMBALIAN: HTTP 200 OK berisi ApiResponse<anonymous> — contoh respons Single Resource.
    /// 
    /// ALASAN MENGGUNAKAN ANONYMOUS TYPE:
    /// `new { Id = 1, Name = "...", Price = ... }` adalah objek anonim C# yang tidak memerlukan definisi class terlebih dahulu.
    /// Berguna untuk data demo atau transformasi data sementara tanpa boilerplate DTO.
    /// </summary>
    [HttpGet("single")]
    public IActionResult GetSingle()
    {
        var product  = new { Id = 1, Name = "PlayStation 5 Slim", Price = 8500000 };
        var response = Common.ApiResponse.Success(product, "Berhasil mendapatkan detail produk.", GetTraceId());
        return Ok(response);
    }

    /// <summary>
    /// FUNGSI METHOD: Mengembalikan daftar produk terpaginasi dalam format PagedData.
    /// PARAMETER:
    ///  - page: Nomor halaman yang diminta (default 1).
    ///  - pageSize: Jumlah item per halaman (default 10).
    /// 
    /// ALUR PAGINASI:
    /// 1. `Enumerable.Range(1, 100)`: Menghasilkan 100 item data dummy dalam memori.
    /// 2. `.Skip((page - 1) * pageSize)`: Melompati item pada halaman-halaman sebelumnya.
    ///    Contoh: page=2, pageSize=10 → Skip(10) untuk memulai dari item ke-11.
    /// 3. `.Take(pageSize)`: Mengambil sejumlah pageSize item dari posisi skip.
    /// 4. `new PagedData<object>(...)`: Membungkus item + metadata paginasi.
    /// </summary>
    [HttpGet("paginated")]
    public IActionResult GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var allProducts = Enumerable.Range(1, 100).Select(i => new
        {
            Id    = i,
            Name  = $"Produk ke-{i}",
            Price = 10000 * i
        }).ToList();

        // Algoritma paginasi manual
        var paginatedItems = allProducts.Skip((page - 1) * pageSize).Take(pageSize);
        var pagedData      = new PagedData<object>(paginatedItems, allProducts.Count, page, pageSize);

        var response = Common.ApiResponse.Success(pagedData, "Berhasil mendapatkan daftar produk terpaginasi.", GetTraceId());
        return Ok(response);
    }

    /// <summary>
    /// FUNGSI METHOD: Mengembalikan respons error standar dalam format ApiResponse.
    /// NILAI KEMBALIAN: HTTP 400 Bad Request berisi ApiResponse dengan Success = false dan daftar error.
    /// 
    /// ALASAN MENGGUNAKAN `Common.ApiResponse.Fail`:
    /// 1. Penggunaan awalan `Common.` mencegah "Namespace Collision" agar C# tidak bingung membedakan 
    ///    antara nama namespace aplikasi (`TesBackendNet.ApiResponse`) dengan nama class (`ApiResponse`).
    /// 2. Format error yang konsisten memungkinkan klien frontend memproses dan menampilkan pesan validasi 
    ///    secara seragam tanpa perlu logika parsing khusus per endpoint.
    /// </summary>
    [HttpGet("error")]
    public IActionResult GetError()
    {
        var errors = new List<string> { "Harga tidak boleh kurang dari 0", "Stok produk tidak boleh kosong" };
        var response = Common.ApiResponse.Fail("Validasi input produk gagal.", errors, GetTraceId());
        return BadRequest(response);
    }
}
