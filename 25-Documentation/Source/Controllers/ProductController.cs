// ============================================================
// ProductController.cs — Controller Ter-dokumentasi Penuh (Swagger)
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.Documentation.Controllers;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

[ApiController]
[Route("api/products")]
[Produces("application/json")] // Mendeklarasikan bahwa API mengembalikan JSON
public class ProductController : ControllerBase
{
    /// <summary>
    /// Mencari detail produk berdasarkan ID unik produk.
    /// </summary>
    /// <remarks>
    /// Contoh Request:
    /// 
    ///     GET /api/products/1
    /// 
    /// </remarks>
    /// <param name="id">ID produk yang dicari (Integer)</param>
    /// <returns>Objek produk jika ditemukan</returns>
    /// <response code="200">Berhasil mengembalikan data produk.</response>
    /// <response code="401">Akses ditolak. Token JWT tidak valid atau kosong.</response>
    /// <response code="404">Produk dengan ID tersebut tidak ditemukan.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetProductById(int id)
    {
        if (id <= 0) return BadRequest("ID tidak valid.");

        if (id == 1)
        {
            var product = new Product { Id = 1, Name = "Monitor UltraWide 34 Inch", Price = 6500000 };
            return Ok(product);
        }

        return NotFound($"Produk dengan ID {id} tidak ditemukan.");
    }
}
