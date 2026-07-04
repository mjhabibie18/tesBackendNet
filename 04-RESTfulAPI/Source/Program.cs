// ============================================================
// Program.cs — Entry Point Aplikasi
// ============================================================
// Di sini kita melakukan konfigurasi:
//   1. API Versioning (Asp.Versioning)
//   2. Content Negotiation (Formatters JSON & XML)
//   3. Entity Framework Core + Seeding Database otomatis
//   4. Swagger generator dengan versi API
// ============================================================

using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.RESTfulAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ── 1. EF Core Setup ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── 2. Content Negotiation & Controllers ──────────────────────
// Secara default, ASP.NET Core hanya me-return JSON.
// Kita tambahkan AddXmlSerializerFormatters agar controller bisa
// mengembalikan XML jika client meminta lewat request header `Accept: application/xml`.
builder.Services.AddControllers(options =>
{
    // Mengizinkan server mengirim status code 406 Not Acceptable jika
    // client meminta format header (Accept) yang tidak disupport server.
    options.ReturnHttpNotAcceptable = true;
})
.AddXmlSerializerFormatters(); // Mendukung XML output formatter

// ── 3. API Versioning Setup ───────────────────────────────────
// Menggunakan library Asp.Versioning.Mvc
builder.Services.AddApiVersioning(options =>
{
    // defaultApiVersion: Jika request tidak menentukan versi, gunakan versi 1.0
    options.DefaultApiVersion = new ApiVersion(1, 0);
    
    // assumeDefaultVersionWhenUnspecified: Pakai defaultApiVersion jika client
    // tidak melampirkan versi di header / query string / url
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // reportApiVersions: Menambahkan header "api-supported-versions" dan 
    // "api-deprecated-versions" pada response HTTP header untuk memberi informasi ke client.
    options.ReportApiVersions = true;

    // apiVersionReader: Cara client mengirimkan versi ke server.
    // Di sini kita mendukung tiga cara (kombinasi reader):
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),                           // 1. URL Path: /api/v1/products
        new QueryStringApiVersionReader("api-version"),             // 2. Query String: ?api-version=1.0
        new HeaderApiVersionReader("X-Version")                     // 3. Custom Header: X-Version: 1.0
    );
})
.AddApiExplorer(options =>
{
    // Format nama grup Swagger untuk API versioning (contoh: 'v1', 'v2')
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ── 4. Swagger Setup ──────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Konfigurasi Swagger Document untuk V1
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TesBackendNet RESTful API - V1",
        Version = "v1",
        Description = "Implementasi RESTful API V1 dengan HATEOAS & Content Negotiation."
    });

    // Konfigurasi Swagger Document untuk V2
    c.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TesBackendNet RESTful API - V2",
        Version = "v2",
        Description = "Implementasi RESTful API V2 (Diskon Promo 10%)."
    });
});

var app = builder.Build();

// ── 5. Database Auto-Migration & Seeding ──────────────────────
// Memastikan database terbuat otomatis saat project running.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated(); // Membuat database jika belum ada + run seed data
}

// ── 6. Middleware Pipeline ────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Menyediakan opsi pilihan versi API di dashboard Swagger
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RESTful API V1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "RESTful API V2");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
