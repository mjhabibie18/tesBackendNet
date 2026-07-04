# Lembar Contekan (Cheat Sheet) — RESTful API, HATEOAS & Versioning

Gunakan rangkuman kode ini untuk implementasi cepat RESTful API tingkat lanjut di ASP.NET Core.

---

## 1. Setup API Versioning (Program.cs)
```csharp
using Asp.Versioning;

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Version")
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

---

## 2. Setup Content Negotiation untuk XML & JSON (Program.cs)
```csharp
builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true; // Tolak request format aneh
})
.AddXmlSerializerFormatters(); // Aktifkan format XML
```

---

## 3. Implementasi Controller dengan Versioning & HATEOAS
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id:int}", Name = nameof(GetProductById))]
    public IActionResult GetProductById(int id)
    {
        var product = new { Id = id, Name = "Laptop" };

        var response = new 
        {
            Data = product,
            Links = new List<object>
            {
                new { href = Url.Link(nameof(GetProductById), new { id }), rel = "self", method = "GET" },
                new { href = Url.Link(nameof(GetProductById), new { id }), rel = "delete", method = "DELETE" }
            }
        };

        return Ok(response);
    }
}
```
