// ============================================================
// DemoResponseController.cs — Controller Demonstrasi Response
// ============================================================

using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TesBackendNet.ApiResponse.Common;

namespace TesBackendNet.ApiResponse.Controllers;

[ApiController]
[Route("api/responses")]
public class DemoResponseController : ControllerBase
{
    // Ambil Trace ID unik dari HttpContext untuk tracing log
    private string GetTraceId() => Activity.Current?.Id ?? HttpContext.TraceIdentifier;

    // 1. Single Resource Response
    [HttpGet("single")]
    public IActionResult GetSingle()
    {
        var product = new { Id = 1, Name = "PlayStation 5 Slim", Price = 8500000 };
        var response = ApiResponse.Success(product, "Berhasil mendapatkan detail produk.", GetTraceId());
        return Ok(response);
    }

    // 2. Paginated List Response
    [HttpGet("paginated")]
    public IActionResult GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var allProducts = Enumerable.Range(1, 100).Select(i => new
        {
            Id = i,
            Name = $"Produk ke-{i}",
            Price = 10000 * i
        }).ToList();

        var paginatedItems = allProducts.Skip((page - 1) * pageSize).Take(pageSize);
        var pagedData = new PagedData<object>(paginatedItems, allProducts.Count, page, pageSize);

        var response = ApiResponse.Success(pagedData, "Berhasil mendapatkan daftar produk terpaginasi.", GetTraceId());
        return Ok(response);
    }

    // 3. Error Response
    [HttpGet("error")]
    public IActionResult GetError()
    {
        var errors = new List<string> { "Harga tidak boleh kurang dari 0", "Stok produk tidak boleh kosong" };
        var response = ApiResponse.Fail("Validasi input produk gagal.", errors, GetTraceId());
        return BadRequest(response);
    }
}
