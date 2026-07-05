// ============================================================
// Nama File: ProductController.cs — Controller Terdokumentasi Penuh untuk Swagger/OpenAPI
// Folder: 25-Documentation/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Documentation):
//    - Tujuan: Mendemonstrasikan cara mendokumentasikan API secara otomatis menggunakan Swagger (Swashbuckle)
//      dan XML Documentation Comments yang dibaca oleh Swagger untuk menghasilkan halaman dokumentasi interaktif.
//    - Kapan Digunakan: Saat membangun API yang akan dikonsumsi oleh tim lain (frontend, mobile, atau klien eksternal).
//      Dokumentasi Swagger menghilangkan kebutuhan dokumentasi manual (Postman collection, Word document, dll.).
//    - Hubungan: XML comments di file ini dibaca oleh Swagger Generator di Program.cs via `c.IncludeXmlComments(xmlPath)`.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengekspos endpoint produk yang dilengkapi dokumentasi penuh untuk halaman Swagger UI.
//    - Mengapa Diperlukan: Menunjukkan best practice penulisan XML documentation comments yang menghasilkan Swagger docs otomatis.
//    - Jika Dihapus: Tidak ada demonstrasi dokumentasi API pada modul ini.
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.Documentation.Controllers;

/// <summary>
/// TUJUAN CLASS: Data model produk yang digunakan sebagai contoh respons API.
/// ALASAN DILETAKKAN DI SINI: Untuk simplifikasi demo tanpa folder Models terpisah.
/// </summary>
public class Product
{
    public int     Id    { get; set; }
    public string  Name  { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

/// <summary>
/// TUJUAN CLASS:
/// Controller yang sepenuhnya didokumentasikan menggunakan XML documentation comments C#.
/// Swagger Generator membaca XML comments ini dan menghasilkan dokumentasi interaktif di /swagger.
/// 
/// CARA KERJA SWAGGER DOCUMENTATION:
/// 1. Setiap XML comment (`/// <summary>`, `/// <param>`, `/// <returns>`) dikompilasi menjadi file .xml.
/// 2. Program.cs mengonfigurasi Swashbuckle untuk membaca file .xml tersebut: `c.IncludeXmlComments(xmlPath)`.
/// 3. Swagger UI menampilkan semua informasi ini dalam antarmuka web interaktif di path /swagger.
/// 
/// [Produces("application/json")]:
/// Atribut ini memberi tahu Swagger bahwa semua endpoint di controller ini menghasilkan format JSON.
/// Swagger akan menampilkan "application/json" sebagai Content-Type di dokumentasi.
/// </summary>
[ApiController]
[Route("api/products")]
[Produces("application/json")] // Deklarasi Content-Type output untuk seluruh controller ini
public class ProductController : ControllerBase
{
    /// <summary>
    /// Mencari detail produk berdasarkan ID unik produk.
    /// </summary>
    /// <remarks>
    /// Contoh Request yang valid:
    /// 
    ///     GET /api/products/1
    ///
    /// Catatan: Hanya ID = 1 yang memiliki data demo pada implementasi ini.
    /// Semua ID lain akan menghasilkan 404 Not Found.
    /// </remarks>
    /// <param name="id">ID produk yang dicari (bilangan bulat positif)</param>
    /// <returns>Objek produk jika ditemukan, atau pesan error jika tidak ada</returns>
    /// <response code="200">Berhasil mengembalikan data produk lengkap.</response>
    /// <response code="400">ID tidak valid (nilai nol atau negatif).</response>
    /// <response code="401">Akses ditolak. Token JWT tidak valid atau kosong.</response>
    /// <response code="404">Produk dengan ID tersebut tidak ditemukan di database.</response>
    /// 
    /// PENJELASAN ATRIBUT [ProducesResponseType]:
    /// Menginformasikan Swagger tentang tipe dan status kode HTTP yang mungkin dikembalikan endpoint ini.
    /// Swagger UI akan menampilkan semua kemungkinan response ini lengkap dengan contoh schema JSON.
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]        // 200: Berhasil dengan body Product
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]               // 401: Tidak terautentikasi
    [ProducesResponseType(StatusCodes.Status404NotFound)]                   // 404: Tidak ditemukan
    public IActionResult GetProductById(int id)
    {
        // Validasi dasar: ID harus bilangan bulat positif
        if (id <= 0) return BadRequest("ID tidak valid.");

        // Data demo: hanya ID 1 yang memiliki data
        if (id == 1)
        {
            var product = new Product { Id = 1, Name = "Monitor UltraWide 34 Inch", Price = 6500000 };
            return Ok(product);
        }

        return NotFound($"Produk dengan ID {id} tidak ditemukan.");
    }
}
