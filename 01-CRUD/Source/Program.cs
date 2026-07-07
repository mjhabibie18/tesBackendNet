// ============================================================
// Program.cs — Entry Point Aplikasi ASP.NET Core
// ============================================================
// File ini adalah titik masuk aplikasi.
// Semua konfigurasi service dan middleware dilakukan di sini.
// ASP.NET Core menggunakan "Minimal Hosting Model" sejak .NET 6+
// yang menggabungkan Startup.cs dan Program.cs menjadi satu file.
// ============================================================

// Menggunakan namespace yang diperlukan
using Microsoft.EntityFrameworkCore;
using TesBackendNet.CRUD.Common;
using TesBackendNet.CRUD.Data;
using TesBackendNet.CRUD.Repositories;
using TesBackendNet.CRUD.Services;
using TesBackendNet.CRUD.Middlewares;

// ── WebApplication.CreateBuilder ──────────────────────────────
// Membuat "builder" yang akan mengkonfigurasi aplikasi.
// Parameter args memungkinkan konfigurasi dari command line.
// Builder ini mengatur:
//   - Dependency Injection Container
//   - Configuration (appsettings.json, environment variables, dll)
//   - Logging
//   - Web server (Kestrel)
var builder = WebApplication.CreateBuilder(args);

// ── Database Context ──────────────────────────────────────────
// Mendaftarkan AppDbContext ke DI Container.
// AddDbContext<T>:
//   - Membuat instance DbContext per HTTP request (Scoped lifetime)
//   - Mengkonfigurasi koneksi ke SQL Server
//   - Memungkinkan injeksi AppDbContext ke class lain
//
// GetConnectionString("DefaultConnection"):
//   - Membaca connection string dari appsettings.json
//   - Key "DefaultConnection" harus ada di "ConnectionStrings" section
//   - TIDAK hardcode connection string di sini!
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // Retry otomatis jika koneksi database gagal sementara
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        }
    )
);

// ── Dependency Injection (DI) ─────────────────────────────────
// Mendaftarkan Repository dan Service ke DI Container.
//
// AddScoped = instance baru dibuat per HTTP request
// Artinya: dalam satu request, semua class yang butuh IProductRepository
// akan mendapat INSTANCE YANG SAMA.
//
// Pola: AddScoped<Interface, Implementation>()
// Kenapa pakai interface?
//   - Loose coupling: ProductController tidak tahu implementasi spesifik
//   - Mudah diganti: bisa swap ProductRepository dengan MockProductRepository
//   - Unit testing: bisa mock interface tanpa database asli
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// ── Controllers ───────────────────────────────────────────────
// Mendaftarkan semua Controller ke aplikasi.
// AddControllers() akan:
//   - Scan semua class yang extend ControllerBase
//   - Register routing untuk setiap action method
//   - Setup JSON serialization (default: System.Text.Json)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Menggunakan camelCase untuk JSON response
        // Contoh: "ProductName" → "productName"
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;

        // Jangan serialize null values untuk response lebih bersih
        // Jika butuh null, hapus baris ini
        // options.JsonSerializerOptions.DefaultIgnoreCondition =
        //     System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ── Swagger / OpenAPI ─────────────────────────────────────────
// AddEndpointsApiExplorer: scan semua endpoint untuk dokumentasi
// AddSwaggerGen: generate Swagger UI
// Swagger memungkinkan testing API langsung dari browser
// tanpa perlu Postman saat development
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title       = "TesBackendNet - CRUD API",
        Version     = "v1",
        Description = "Contoh implementasi CRUD dengan ASP.NET Core, EF Core, Repository Pattern, dan Service Layer.",
        Contact     = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "TesBackendNet",
            Url  = new Uri("https://github.com/yourusername/tesBackendNet")
        }
    });

    // Include XML comments ke Swagger (dari /// komentar di Controller)
    // Uncomment jika sudah tambahkan <GenerateDocumentationFile>true</GenerateDocumentationFile>
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // options.IncludeXmlComments(xmlPath);
});

// ── CORS (Cross-Origin Resource Sharing) ─────────────────────
// Mengizinkan request dari domain/port lain (misal: frontend React/Vue)
// Ini penting jika frontend dan backend berbeda domain/port
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // Development: izinkan semua origin
        // Production: ganti dengan domain spesifik
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ── Build Application ─────────────────────────────────────────
// Setelah semua service terdaftar, build aplikasi.
// Setelah build(), tidak bisa tambah service lagi.
var app = builder.Build();

// ── Auto-apply Migrations ─────────────────────────────────────
// Otomatis apply migration saat startup (khusus development)
// Di production, sebaiknya migration dijalankan manual atau di CI/CD
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middleware Pipeline ───────────────────────────────────────
// Middleware adalah komponen yang memproses setiap HTTP request.
// URUTAN middleware PENTING! Request diproses dari atas ke bawah.

// Development middleware
if (app.Environment.IsDevelopment())
{
    // Swagger UI: tersedia di /swagger
    // Hanya aktif di Development untuk keamanan
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CRUD API v1");
        // Swagger UI sebagai halaman utama (opsional)
        // options.RoutePrefix = string.Empty;
    });

    // Detail error di browser saat development
    app.UseDeveloperExceptionPage();
}

// Global Exception Handler
// Harus dipasang sebelum middleware lain agar bisa catch semua error
app.UseMiddleware<GlobalExceptionMiddleware>();

// Redirect HTTP ke HTTPS
app.UseHttpsRedirection();

// CORS — harus sebelum Authorization
app.UseCors("AllowAll");

// Authorization middleware
// Meski modul ini belum pakai auth, ini adalah posisi yang benar
app.UseAuthorization();

// Map Controllers: hubungkan HTTP request ke Controller action
app.MapControllers();

// Jalankan aplikasi
app.Run();
