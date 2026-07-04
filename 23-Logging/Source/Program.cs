// ============================================================
// Program.cs — Entry Point Project Logging
// ============================================================

using Serilog;
using Serilog.Events;

// ── 1. Setup Logger Konfigurasi Serilog ────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // Level minimal log umum
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // Filter log internal framework
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    // A. Sink Konsol: Tampilan warna-warni yang rapi
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    // B. Sink File: Penyimpanan log fisik harian dengan rolling interval
    .WriteTo.File(
        path: "logs/app-log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30) // Simpan maksimal 30 hari log terakhir
    .CreateLogger();

try
{
    Log.Information("Memulai web host aplikasi...");

    var builder = WebApplication.CreateBuilder(args);

    // ── 2. Integrasikan Serilog ke Host .NET ────────────────────
    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host aplikasi berhenti secara tidak wajar.");
}
finally
{
    Log.CloseAndFlush(); // Tutup koneksi log file
}
