// ============================================================
// Program.cs — Entry Point Project Caching
// ============================================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── 1. Register In-Memory Caching (IMemoryCache) ──────────────
builder.Services.AddMemoryCache();

// ── 2. Register Distributed Caching (IDistributedCache) ───────
// Catatan: Di production, kita menggunakan Redis.
// Agar project ini langsung jalan (out-of-the-box) tanpa harus menyalakan Redis,
// kita gunakan AddDistributedMemoryCache() sebagai mock in-memory distributed storage.
// Jika ingin menggunakan Redis sungguhan, un-comment bagian AddStackExchangeRedisCache di bawah:

builder.Services.AddDistributedMemoryCache();

/*
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TesBackendNet_";
});
*/

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
