// ============================================================
// DemoController.cs — Controller Demonstrasi DI & Filter
// ============================================================

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TesBackendNet.Framework.Services;

namespace TesBackendNet.Framework.Controllers;

[ApiController]
[Route("api/framework")]
public class DemoController : ControllerBase
{
    private readonly ITransientService _transient1;
    private readonly ITransientService _transient2;
    private readonly IScopedService _scoped1;
    private readonly IScopedService _scoped2;
    private readonly ISingletonService _singleton1;
    private readonly ISingletonService _singleton2;

    // Inject masing-masing service 2 kali untuk melihat perbandingan Operation ID (GUID)
    public DemoController(
        ITransientService transient1, ITransientService transient2,
        IScopedService scoped1, IScopedService scoped2,
        ISingletonService singleton1, ISingletonService singleton2)
    {
        _transient1 = transient1;
        _transient2 = transient2;
        _scoped1 = scoped1;
        _scoped2 = scoped2;
        _singleton1 = singleton1;
        _singleton2 = singleton2;
    }

    // 1. Endpoint DI Lifetimes Test
    [HttpGet("lifetimes")]
    public IActionResult GetLifetimes()
    {
        return Ok(new
        {
            Transient = new
            {
                Instance1 = _transient1.GetOperationId(),
                Instance2 = _transient2.GetOperationId(),
                Notes = "Transient menghasilkan ID yang berbeda di setiap inject (selalu berbeda)"
            },
            Scoped = new
            {
                Instance1 = _scoped1.GetOperationId(),
                Instance2 = _scoped2.GetOperationId(),
                Notes = "Scoped menghasilkan ID yang SAMA di request HTTP yang sama (sama per request)"
            },
            Singleton = new
            {
                Instance1 = _singleton1.GetOperationId(),
                Instance2 = _singleton2.GetOperationId(),
                Notes = "Singleton menghasilkan ID yang SAMA di seluruh siklus hidup server (selalu sama)"
            }
        });
    }

    // 2. Endpoint Filter Validation Test
    [HttpPost("validate")]
    public IActionResult TestValidation([FromBody] SampleModel model)
    {
        return Ok(new { Message = "Model valid!", Data = model });
    }
}

public class SampleModel
{
    [Required(ErrorMessage = "Nama wajib diisi")]
    [MinLength(5, ErrorMessage = "Nama minimal 5 karakter")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 100, ErrorMessage = "Nilai harus di rentang 1-100")]
    public int Value { get; set; }
}
