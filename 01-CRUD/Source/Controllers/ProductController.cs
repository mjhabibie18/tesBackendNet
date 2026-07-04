// ============================================================
// ProductController.cs — Controller Layer
// ============================================================
// Controller adalah pintu masuk HTTP request ke aplikasi.
// Tanggung jawab Controller:
//   1. Menerima HTTP request
//   2. Validasi input dasar (via DataAnnotations di DTO)
//   3. Memanggil Service
//   4. Mengembalikan HTTP response yang sesuai
//
// Controller TIDAK boleh:
//   - Mengandung business logic
//   - Mengakses database langsung
//   - Melakukan transformasi data yang kompleks
//
// Mengapa [ApiController]?
//   - Automatic model validation (400 jika DTO tidak valid)
//   - Automatic binding dari request body, query string, route
//   - Return standardized ProblemDetails untuk error
//
// Mengapa extend ControllerBase (bukan Controller)?
//   - Controller = ControllerBase + View support (untuk MVC dengan Razor)
//   - ControllerBase = hanya untuk API (tidak ada View)
//   - API tidak butuh View, jadi pakai ControllerBase agar lebih ringan
// ============================================================

using Microsoft.AspNetCore.Mvc;
using TesBackendNet.CRUD.Common;
using TesBackendNet.CRUD.DTO;
using TesBackendNet.CRUD.Services;

namespace TesBackendNet.CRUD.Controllers;

/// <summary>
/// Controller untuk mengelola data Product.
/// Endpoint: /api/products
///
/// CRUD Operations:
/// - GET    /api/products              → GetAll (dengan pagination, search, filter, sort)
/// - GET    /api/products/{id}         → GetById
/// - POST   /api/products              → Create
/// - PUT    /api/products/{id}         → Update
/// - DELETE /api/products/{id}         → Delete (Soft)
/// </summary>
///
/// [ApiController]:
/// - Aktifkan automatic model validation
/// - Return 400 otomatis jika DTO tidak valid (tanpa perlu if (!ModelState.IsValid))
/// - Bind parameter otomatis dari route, query, body
///
/// [Route("api/[controller]")]:
/// - [controller] = nama class tanpa "Controller" suffix = "products" (lowercase otomatis)
/// - Jadi URL menjadi: /api/products
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    // ── Dependencies ──────────────────────────────────────────
    // Menggunakan interface IProductService, bukan ProductService langsung.
    // Ini adalah Dependency Inversion Principle.
    private readonly IProductService _service;

    // ILogger untuk logging (optional tapi good practice)
    private readonly ILogger<ProductController> _logger;

    // ── Constructor Injection ─────────────────────────────────
    // ASP.NET Core DI Container otomatis inject IProductService
    // yang sudah didaftarkan di Program.cs
    public ProductController(IProductService service, ILogger<ProductController> logger)
    {
        _service = service;
        _logger  = logger;
    }

    // ================================================================
    // GET /api/products
    // GET /api/products?search=laptop
    // GET /api/products?page=2&pageSize=5
    // GET /api/products?sortBy=price&sortDesc=true
    // GET /api/products?minPrice=100000&maxPrice=5000000
    // ================================================================

    /// <summary>
    /// Mengambil semua product dengan dukungan pagination, search, filter, dan sorting.
    /// </summary>
    /// <param name="query">Parameter query (search, sort, page, filter)</param>
    /// <returns>Daftar product dengan metadata pagination</returns>
    ///
    /// [HttpGet]: endpoint ini merespons HTTP GET request
    /// [FromQuery]: query dipopulate dari query string URL
    ///
    /// Mengapa return IActionResult?
    ///   - IActionResult = abstraksi untuk berbagai HTTP response
    ///   - Bisa return Ok(), NotFound(), BadRequest(), dll
    ///   - Lebih fleksibel dari return tipe langsung
    ///
    /// Mengapa async Task?
    ///   - async: method ini adalah asynchronous
    ///   - Task: wrapper untuk operasi async
    ///   - Ini memungkinkan thread server tidak "blocked" saat menunggu database
    ///   - Tanpa async: thread terblokir → server tidak bisa handle request lain
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProductResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
    {
        _logger.LogInformation("GetAll products - Page: {Page}, PageSize: {PageSize}, Search: {Search}",
            query.Page, query.PageSize, query.Search);

        var result = await _service.GetAllAsync(query);

        // Ok() = HTTP 200 dengan response body
        // ApiResponse.Success() membungkus data dalam format standar
        return Ok(ApiResponse.Success(result, "Berhasil mengambil data produk"));
    }

    // ================================================================
    // GET /api/products/5
    // ================================================================

    /// <summary>
    /// Mengambil satu product berdasarkan ID.
    /// </summary>
    /// <param name="id">ID product</param>
    ///
    /// [HttpGet("{id:int}")]:
    ///   - "{id}" = route parameter (bagian dari URL path)
    ///   - ":int" = constraint: hanya match jika id adalah integer
    ///   - Jika request GET /api/products/abc → tidak match, return 404
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("GetById product - ID: {Id}", id);

        var result = await _service.GetByIdAsync(id);

        // Jika null = tidak ditemukan → return 404
        if (result == null)
        {
            // NotFound() = HTTP 404
            return NotFound(ApiResponse.Fail($"Product dengan ID {id} tidak ditemukan."));
        }

        return Ok(ApiResponse.Success(result));
    }

    // ================================================================
    // POST /api/products
    // Body: { "name": "...", "price": 0, "stock": 0 }
    // ================================================================

    /// <summary>
    /// Membuat product baru.
    /// </summary>
    /// <param name="dto">Data product baru</param>
    ///
    /// [HttpPost]: merespons HTTP POST request
    /// [FromBody]: dto dipopulate dari request body (JSON)
    ///
    /// Mengapa return 201 Created (bukan 200 OK)?
    ///   - HTTP spec: 201 = resource baru berhasil dibuat
    ///   - 201 juga include Location header yang menunjuk ke resource baru
    ///   - Best practice untuk POST yang membuat resource
    ///
    /// CreatedAtAction():
    ///   - Membuat HTTP 201 response
    ///   - Menambahkan Location header: /api/products/{id}
    ///   - Berguna agar client tahu URL resource yang baru dibuat
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        // Catatan: Karena ada [ApiController], validasi DataAnnotations di DTO
        // sudah otomatis dilakukan. Jika Name kosong/tidak valid,
        // ASP.NET Core langsung return 400 Bad Request SEBELUM masuk ke sini.

        _logger.LogInformation("Create product - Name: {Name}", dto.Name);

        var result = await _service.CreateAsync(dto);

        // CreatedAtAction: return 201 dengan Location header
        // nameof(GetById) = nama action untuk generate URL
        // new { id = result.Id } = route values untuk generate URL
        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Id },
            ApiResponse.Success(result, "Product berhasil dibuat"));
    }

    // ================================================================
    // PUT /api/products/5
    // Body: { "name": "...", "price": 0, "stock": 0 }
    // ================================================================

    /// <summary>
    /// Mengupdate product yang sudah ada (PUT = replace seluruh resource).
    /// </summary>
    /// <param name="id">ID product yang akan diupdate</param>
    /// <param name="dto">Data baru</param>
    ///
    /// PUT vs PATCH:
    ///   PUT   = kirim SEMUA field, replace seluruh resource
    ///   PATCH = kirim hanya field yang berubah, partial update
    ///
    /// Mengapa return 200 (bukan 204)?
    ///   - 204 = No Content: berhasil tapi tidak ada response body
    ///   - 200 = OK: berhasil dengan response body (updated data)
    ///   - Return updated data lebih informatif untuk client
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ProductResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        _logger.LogInformation("Update product - ID: {Id}, Name: {Name}", id, dto.Name);

        var result = await _service.UpdateAsync(id, dto);
        return Ok(ApiResponse.Success(result, "Product berhasil diupdate"));
    }

    // ================================================================
    // DELETE /api/products/5
    // ================================================================

    /// <summary>
    /// Menghapus product (soft delete — tidak benar-benar dihapus dari database).
    /// </summary>
    /// <param name="id">ID product yang akan dihapus</param>
    ///
    /// Mengapa return 200 (bukan 204)?
    ///   - 204 = tidak ada response body
    ///   - 200 = ada response body dengan pesan konfirmasi
    ///   - Untuk API yang membutuhkan pesan konfirmasi, 200 lebih baik
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Delete (soft) product - ID: {Id}", id);

        await _service.DeleteAsync(id);

        // Return 200 dengan pesan konfirmasi
        return Ok(ApiResponse.Success("Product berhasil dihapus"));
    }
}
