// ============================================================
// Program.cs — Entry Point Project Validasi
// ============================================================

using FluentValidation;
using TesBackendNet.Validation.Validators;

var builder = WebApplication.CreateBuilder(args);

// ── Register Controllers ──────────────────────────────────────
builder.Services.AddControllers();

// ── Register FluentValidation ─────────────────────────────────
// Mencari semua class validator di assembly ini secara otomatis
builder.Services.AddValidatorsFromAssemblyContaining<UserRegisterValidator>();

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
