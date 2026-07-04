// ============================================================
// DesignDemoController.cs — Controller Uji Coba Skema Database Design
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.DatabaseDesign.Data;
using TesBackendNet.DatabaseDesign.Models;

namespace TesBackendNet.DatabaseDesign.Controllers;

[ApiController]
[Route("api/design")]
public class DesignDemoController : ControllerBase
{
    private readonly AppDbContext _context;

    public DesignDemoController(AppDbContext context)
    {
        _context = context;
    }

    // 1. Ambil List User
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users.Include(u => u.Orders).ToListAsync();
        return Ok(users);
    }

    // 2. Insert User (Uji Coba CHECK constraint & UNIQUE constraint)
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] UserDto dto)
    {
        var newUser = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Age = dto.Age
        };

        try
        {
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Created("", newUser);
        }
        catch (DbUpdateException ex)
        {
            // Menangkap error constraint pelanggaran UNIQUE atau CHECK di database SQL Server
            return BadRequest(new
            {
                Success = false,
                Message = "Gagal membuat User. Terjadi pelanggaran Constraint database (UNIQUE / CHECK / NOT NULL).",
                Details = ex.InnerException?.Message ?? ex.Message
            });
        }
    }
}

public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
}
