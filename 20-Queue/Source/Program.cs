// ============================================================
// Program.cs — Entry Point Project Queue
// ============================================================

using Hangfire;
using TesBackendNet.Queue.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Register Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

// ── 1. Registrasi Hangfire Core & In-Memory Storage ────────────
builder.Services.AddHangfire(config =>
{
    // Menggunakan In-Memory Storage untuk demo instan (tanpa perlu SQL Server)
    config.UseInMemoryStorage();
    
    // Jika di production, un-comment bagian UseSqlServerStorage berikut:
    // config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ── 2. Registrasi Server Worker Hangfire ───────────────────────
// Service background yang akan memantau queue dan memproses task
builder.Services.AddHangfireServer();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ── 3. Aktifkan Dashboard Hangfire ────────────────────────────
// Akses dashboard visual di: http://localhost:5000/hangfire
app.UseHangfireDashboard("/hangfire");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
