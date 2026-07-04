// ============================================================
// Program.cs — Entry Point
// ============================================================

using TesBackendNet.ErrorHandling.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── PENTING: Daftarkan Exception Middleware Paling Awal ──────
// Agar middleware ini dapat membungkus seluruh pipeline middleware lainnya.
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
