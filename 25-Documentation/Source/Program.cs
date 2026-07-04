// ============================================================
// Program.cs — Entry Point Project OpenAPI/Swagger Documentation
// ============================================================

using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── 1. Konfigurasi Swagger & OpenAPI Info ─────────────────────
builder.Services.AddSwaggerGen(options =>
{
    // A. Meta info dokumentasi API
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TesBackendNet OpenAPI",
        Version = "v1",
        Description = "API Portofolio & Pembelajaran Backend ASP.NET Core",
        Contact = new OpenApiContact
        {
            Name = "Muhammad Habibie",
            Email = "mjhabibie18@gmail.com",
            Url = new Uri("https://github.com/mjhabibie18")
        }
    });

    // B. Baca file komentar XML (untuk summary deskripsi endpoint di UI)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // C. Konfigurasi Security Definition untuk tombol "Authorize" (JWT Lock)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan token JWT Anda secara langsung. Contoh: 'eyJhbGciOi...'"
    });

    // D. Terapkan requirement security global ke Swagger
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ── 2. Konfigurasi Endpoint Swagger UI ────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TesBackendNet API v1");
        
        // Mengubah route prefix agar dokumentasi diakses melalui url: /docs
        options.RoutePrefix = "docs"; 
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
