// ============================================================
// ConfigController.cs — Controller Akses Konfigurasi
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TesBackendNet.EnvironmentDemo.Options;

namespace TesBackendNet.EnvironmentDemo.Controllers;

[ApiController]
[Route("api/config")]
public class ConfigController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly JwtSettings _jwtSettings;

    // Inject JwtSettings secara strongly-typed menggunakan IOptions<T>
    public ConfigController(IWebHostEnvironment env, IConfiguration config, IOptions<JwtSettings> jwtSettings)
    {
        _env = env;
        _config = config;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpGet("info")]
    public IActionResult GetConfigInfo()
    {
        return Ok(new
        {
            ActiveEnvironment = _env.EnvironmentName,
            AppFeatures = new
            {
                EnablePremium = _config.GetValue<bool>("AppFeatures:EnablePremiumFeatures"),
                Gateway = _config["AppFeatures:PaymentGateway"]
            },
            JwtSettings = new
            {
                // JANGAN PERNAH me-log / mengekspos secret key asli di API produksi.
                // Disini kita sensor untuk tujuan keamanan.
                MaskedSecretKey = _jwtSettings.SecretKey.Substring(0, 5) + "...[MASKED]",
                _jwtSettings.ExpiryDays
            }
        });
    }
}
