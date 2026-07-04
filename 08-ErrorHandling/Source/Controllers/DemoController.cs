// ============================================================
// DemoController.cs — Controller Demo Pemicu Exception
// ============================================================

using Microsoft.AspNetCore.Mvc;
using TesBackendNet.ErrorHandling.Exceptions;

namespace TesBackendNet.ErrorHandling.Controllers;

[ApiController]
[Route("api/demo")]
public class DemoController : ControllerBase
{
    [HttpGet("ok")]
    public IActionResult GetOk() => Ok(new { Message = "Berhasil mengakses endpoint normal!" });

    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        throw new BadRequestException("Data input tidak valid atau tidak lengkap.");
    }

    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        throw new NotFoundException("Resource dengan ID tersebut tidak ditemukan.");
    }

    [HttpGet("conflict")]
    public IActionResult GetConflict()
    {
        throw new ConflictException("Data dengan nama tersebut sudah ada di database.");
    }

    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        // Menyimulasikan pembagian dengan nol
        int a = 10;
        int b = 0;
        int c = a / b;
        return Ok(c);
    }
}
