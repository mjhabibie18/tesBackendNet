// ============================================================
// Program.cs — Entry Point Project Security
// ============================================================

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using TesBackendNet.Security.Data;

var builder = WebApplication.CreateBuilder(args);

// ── 1. EF Core Setup ──────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

// ── 2. CORS Setup (Cross-Origin Resource Sharing) ─────────────
builder.Services.AddCors(options =>
{
    // Policy Development: melonggarkan CORS untuk development frontend lokal
    options.AddPolicy("DevelopmentCors", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());

    // Policy Production: membatasi domain secara ketat
    options.AddPolicy("ProductionCors", policy =>
        policy.WithOrigins("https://domainresmi.com")
              .WithMethods("GET", "POST", "PUT", "DELETE")
              .WithHeaders("Content-Type", "Authorization")
              .AllowCredentials());
});

// ── 3. .NET 8 Rate Limiting Setup ─────────────────────────────
builder.Services.AddRateLimiter(options =>
{
    // Menambahkan policy "FixedWindow"
    // Membatasi maksimal 5 request dalam jendela waktu 10 detik per IP Address / Client
    options.AddFixedWindowLimiter("FixedWindow", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueLimit = 0; // Request yang melebihi limit langsung ditolak tanpa antrian
    });

    // Mengembalikan status code 429 Too Many Requests jika terkena rate limit
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── 4. Seed Database ──────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

// ── 5. Security Middleware Pipeline ───────────────────────────

// A. HSTS & HTTPS Redirection
if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // Mengharuskan client mengakses via HTTPS saja
}
app.UseHttpsRedirection();

// B. Custom Security Headers Middleware
// Menambahkan header perlindungan standar OWASP ke semua HTTP response
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    await next();
});

// C. CORS Middleware
app.UseCors("DevelopmentCors");

// D. Rate Limiting Middleware
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
