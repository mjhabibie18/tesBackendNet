# 📖 25 — Documentation (Swagger/OpenAPI)

## Swagger / OpenAPI

Swagger (OpenAPI) adalah standar untuk mendokumentasikan REST API secara otomatis.

---

## Setup Dasar

```bash
dotnet add package Swashbuckle.AspNetCore
```

```csharp
// Program.cs
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "My API",
        Version     = "v1",
        Description = "API documentation",
        Contact     = new OpenApiContact
        {
            Name  = "Developer",
            Email = "dev@example.com"
        }
    });
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    options.RoutePrefix = "docs"; // Akses di /docs
});
```

---

## XML Comments → Swagger

```xml
<!-- Di .csproj -->
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

```csharp
/// <summary>
/// Mengambil semua product dengan pagination.
/// </summary>
/// <param name="page">Nomor halaman (default: 1)</param>
/// <param name="pageSize">Jumlah per halaman (default: 10, max: 100)</param>
/// <returns>Daftar product dengan metadata pagination</returns>
/// <response code="200">Berhasil mengambil data</response>
/// <response code="401">Tidak terautentikasi</response>
[HttpGet]
[ProducesResponseType(typeof(ApiResponse<PagedResult<ProductDto>>), 200)]
[ProducesResponseType(401)]
public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10) { ... }
```

---

## Swagger dengan JWT

```csharp
builder.Services.AddSwaggerGen(options =>
{
    // Tombol "Authorize" di Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type        = SecuritySchemeType.Http,
        Scheme      = "bearer",
        BearerFormat = "JWT",
        Description = "Masukkan JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
```

---

## Postman Collection

Swagger JSON bisa diimport ke Postman:
1. Buka Postman → Import
2. URL: `https://localhost:PORT/swagger/v1/swagger.json`
3. Generate collection otomatis!

---

## ✅ Best Practice

1. **Dokumentasi setiap endpoint**: summary, parameter, response
2. **Gunakan ProducesResponseType**: Swagger tahu semua kemungkinan response
3. **Sembunyikan di production**: Swagger hanya untuk development/staging
4. **Version API**: v1, v2 terpisah di Swagger
