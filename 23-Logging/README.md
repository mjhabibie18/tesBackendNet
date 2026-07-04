# 📝 23 — Logging

## Mengapa Logging Penting?

Logging adalah "mata" aplikasi di production. Tanpa logging, debugging masalah production = buta.

---

## Logging Built-in ASP.NET Core

```csharp
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
        => _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        // Level logging: Trace, Debug, Information, Warning, Error, Critical
        _logger.LogInformation("Request GetAll products dari IP: {IP}",
            HttpContext.Connection.RemoteIpAddress);

        try
        {
            var result = await _service.GetAllAsync();
            _logger.LogDebug("Berhasil ambil {Count} products", result.Data.Count);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saat GetAll products");
            throw;
        }
    }
}
```

---

## Serilog (Recommended)

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Seq   # Opsional: Seq log server
```

```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)
    .CreateLogger();

builder.Host.UseSerilog();
```

---

## Log Levels

| Level | Kapan Digunakan |
|-------|----------------|
| **Trace** | Sangat detail, development only |
| **Debug** | Debugging information |
| **Information** | Normal app flow (request masuk, dll) |
| **Warning** | Kondisi tidak ideal tapi tidak error |
| **Error** | Error yang ter-handle |
| **Critical** | Fatal error, app mungkin crash |

---

## Structured Logging

```csharp
// ❌ String interpolation (tidak structured)
_logger.LogInformation($"User {userId} login pada {DateTime.Now}");

// ✅ Structured logging (bisa di-query!)
_logger.LogInformation("User {UserId} login pada {LoginTime}", userId, DateTime.Now);
// → Log entry punya properties: UserId=123, LoginTime=2024-01-01
```

---

## ✅ Best Practice

1. **Jangan log sensitive data**: password, token, credit card
2. **Gunakan structured logging**: bukan string concatenation
3. **Log correlation ID**: track satu request di semua log
4. **Set level yang tepat**: jangan semua `Debug` di production
5. **Centralized logging**: Seq, ELK Stack, Application Insights

---

## 🎤 Tips Interview

**Q: "Apa bedanya log Warning dan Error?"**
```
Warning: sesuatu tidak ideal tapi aplikasi masih bisa lanjut
         Contoh: rate limit hampir tercapai, database slow
Error:   sesuatu gagal tapi ter-handle dengan baik
         Contoh: user tidak ditemukan, validasi gagal
Critical: aplikasi tidak bisa lanjut
         Contoh: database tidak bisa diakses, out of memory
```
