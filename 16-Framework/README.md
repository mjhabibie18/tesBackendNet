# 🧰 16 — Framework (ASP.NET Core)

## ASP.NET Core Overview

ASP.NET Core adalah framework open-source Microsoft untuk membangun web API, web apps, dan microservices.

---

## Request Pipeline

```
HTTP Request → Kestrel (Web Server)
    → Middleware 1 (Exception Handler)
    → Middleware 2 (HTTPS Redirect)
    → Middleware 3 (Static Files)
    → Middleware 4 (Routing)
    → Middleware 5 (Authentication)
    → Middleware 6 (Authorization)
    → Middleware 7 (Controllers)
    → Endpoint (Action Method)
    ← Response (balik keatas)
```

---

## Middleware

```csharp
// Custom middleware
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Request: {Method} {Path}", 
            context.Request.Method, context.Request.Path);

        await _next(context); // Teruskan ke middleware berikutnya

        _logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
    }
}

// Register di Program.cs
app.UseMiddleware<RequestLoggingMiddleware>();
```

---

## Dependency Injection Lifetimes

```
Singleton:  Satu instance untuk seluruh aplikasi
            Contoh: IMemoryCache, IConfiguration, LoggerFactory

Scoped:     Satu instance per HTTP request
            Contoh: DbContext, Repository, Service (paling umum)

Transient:  Instance baru setiap kali di-inject
            Contoh: Lightweight, stateless services
```

```csharp
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
```

---

## Configuration System

```csharp
// Baca dari appsettings.json
var connectionString = builder.Configuration.GetConnectionString("Default");
var jwtKey          = builder.Configuration["Jwt:SecretKey"];
var maxRetry        = builder.Configuration.GetValue<int>("MaxRetry", defaultValue: 3);

// Options Pattern (Strongly Typed)
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Inject via IOptions<T>
public class MyService
{
    private readonly JwtSettings _settings;
    public MyService(IOptions<JwtSettings> settings) => _settings = settings.Value;
}
```

---

## Filter

```csharp
// Action Filter: dieksekusi sebelum/sesudah action method
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

// Register global
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});
```

---

## 🎤 Tips Interview

**Q: "Apa bedanya Middleware dan Filter?"**
```
Middleware: beroperasi pada level HTTP pipeline (request/response)
Filter: beroperasi pada level MVC (setelah routing ke controller)

Gunakan Middleware untuk: auth, logging, CORS, exception handling
Gunakan Filter untuk: validasi model, cache, authorization per controller
```
