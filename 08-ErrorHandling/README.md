# 🚨 08 — Error Handling

## Strategi Error Handling

Error handling yang baik membuat API lebih:
- Informatif untuk developer
- Aman (tidak expose detail internal)
- Konsisten (format error yang sama)

---

## Jenis Error

| Kategori | Contoh | HTTP Status |
|----------|--------|-------------|
| **Validation Error** | Field kosong, format salah | 400, 422 |
| **Not Found** | ID tidak ada | 404 |
| **Unauthorized** | Tidak ada/salah token | 401 |
| **Forbidden** | Tidak punya hak | 403 |
| **Conflict** | Data duplikat | 409 |
| **Server Error** | Unexpected exception | 500 |

---

## Global Exception Middleware

```csharp
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        ctx.Response.ContentType = "application/json";

        var (statusCode, message) = ex switch
        {
            KeyNotFoundException      => (404, ex.Message),
            InvalidOperationException => (409, ex.Message),
            ArgumentException         => (400, ex.Message),
            UnauthorizedAccessException => (401, "Akses ditolak"),
            _                         => (500, "Terjadi kesalahan server")
        };

        ctx.Response.StatusCode = statusCode;
        await ctx.Response.WriteAsJsonAsync(new
        {
            success   = false,
            message,
            timestamp = DateTime.UtcNow
        });
    }
}

// Register di Program.cs (PERTAMA dalam pipeline!)
app.UseMiddleware<GlobalExceptionMiddleware>();
```

---

## Custom Exception Classes

```csharp
// Base custom exception
public abstract class AppException : Exception
{
    public int StatusCode { get; }
    protected AppException(string message, int statusCode) : base(message)
        => StatusCode = statusCode;
}

// Specific exceptions
public class NotFoundException : AppException
{
    public NotFoundException(string name, int id)
        : base($"{name} dengan ID {id} tidak ditemukan", 404) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, 409) { }
}

public class ValidationException : AppException
{
    public List<string> Errors { get; }
    public ValidationException(List<string> errors)
        : base("Validasi gagal", 422)
        => Errors = errors;
}

// Penggunaan
throw new NotFoundException("Product", id);
throw new ConflictException($"Product '{dto.Name}' sudah ada");
```

---

## Problem Details (RFC 7807)

ASP.NET Core mendukung standar Problem Details:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Not Found",
  "status": 404,
  "detail": "Product dengan ID 5 tidak ditemukan",
  "instance": "/api/products/5"
}
```

```csharp
// Di Program.cs
builder.Services.AddProblemDetails();

// Di Controller
return Problem(
    detail: $"Product dengan ID {id} tidak ditemukan",
    statusCode: 404,
    title: "Not Found"
);
```

---

## ✅ Best Practice

1. **Global handler**: tangkap semua exception di satu tempat
2. **Log detail**: log stack trace untuk developer
3. **Response aman**: jangan expose stack trace ke client
4. **Pesan bermakna**: error message yang membantu debugging
5. **Custom exceptions**: buat hierarchy exception yang meaningful
6. **Konsisten**: format error response selalu sama

---

## 🎤 Tips Interview

**Q: "Bagaimana cara handle exception di ASP.NET Core?"**
```
1. Global Exception Middleware (recommended): tangkap semua
2. UseExceptionHandler(): built-in, redirect ke /error endpoint
3. try-catch di Controller: terlalu verbose, tidak DRY
4. IExceptionFilter: filter untuk exception spesifik
```
