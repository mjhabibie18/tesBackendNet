// ============================================================
// Program.cs — Entry Point Project Microservices
// ============================================================

using TesBackendNet.MicroservicesDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ── Register Resilient HttpClient ─────────────────────────────
// Mengintegrasikan ProductServiceClient dengan HttpClient ter-resilience.
// Menggunakan AddStandardResilienceHandler() bawaan .NET 8 untuk otomatisasi
// Retry, Circuit Breaker, Rate Limiter, dan Timeout.
builder.Services.AddHttpClient<ProductServiceClient>(client =>
{
    // Arahkan ke base url Product Service (Untuk demo kita arahkan ke localhost port aktif)
    client.BaseAddress = new Uri("http://localhost:5000/");
})
.AddStandardResilienceHandler(options =>
{
    // Konfigurasi retry attempts
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromSeconds(1);
});

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
