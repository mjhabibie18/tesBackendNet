// ============================================================
// Program.cs — Entry Point Project ORM
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.ORM.Data;

var builder = WebApplication.CreateBuilder(args);

// ── Register EF Core DbContext ────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
    // Konfigurasi JSON serializer agar tidak terjadi infinite looping
    // saat merender object relasi sirkular (Category -> Product -> Category)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── Auto-Migrate / Ensure Created ─────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
