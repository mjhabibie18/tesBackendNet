// ============================================================
// Program.cs — Entry Point Project Environment & Configuration
// ============================================================

using TesBackendNet.EnvironmentDemo.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── 1. Bind & Validasi Konfigurasi (Options Pattern) ──────────
// Melakukan validasi data annotations saat inisialisasi aplikasi (Fail Fast)
builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart(); // Fail-fast: Crash saat startup jika setting invalid

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Menampilkan environment aktif di log konsol
Console.WriteLine($"[Startup] Aplikasi berjalan pada environment: {app.Environment.EnvironmentName}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
