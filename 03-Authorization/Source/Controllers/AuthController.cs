// ============================================================
// AuthController.cs — Controller Authentication Utama
// ============================================================

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesBackendNet.Authorization.DTO;
using TesBackendNet.Authorization.Services;

namespace TesBackendNet.Authorization.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return Ok(new { success = true, message = "Registrasi berhasil", data = result });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(dto, ipAddress);
        return Ok(new { success = true, message = "Login berhasil", data = result });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ipAddress);
        return Ok(new { success = true, message = "Token berhasil diperbarui", data = result });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        await _authService.LogoutAsync(dto.RefreshToken);
        return Ok(new { success = true, message = "Logout berhasil" });
    }

    // [Authorize] artinya HANYA orang yang login (token JWT valid) yang bisa akses endpoint ini.
    // Tidak peduli role-nya apa (Admin/User), selama token valid, bisa diakses.
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { success = false, message = "Token tidak valid" });

        var result = await _authService.GetProfileAsync(userId);
        if (result == null)
            return NotFound(new { success = false, message = "User tidak ditemukan" });

        return Ok(new { success = true, data = result });
    }
}
