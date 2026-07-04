// ============================================================
// Program.cs — Entry Point Project Framework
// ============================================================

using TesBackendNet.Framework.Filters;
using TesBackendNet.Framework.Middlewares;
using TesBackendNet.Framework.Services;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Register Controllers & Global Action Filter ────────────
builder.Services.AddControllers(options =>
{
    // Mendaftarkan filter secara global untuk seluruh controller
    options.Filters.Add<ValidationFilter>();
});

// ── 2. Register DI Lifetimes ──────────────────────────────────
// Kita meregistrasikan satu class yang sama dengan lifetime berbeda
builder.Services.AddTransient<ITransientService, LifetimeDemoService>();
builder.Services.AddScoped<IScopedService, LifetimeDemoService>();
builder.Services.AddSingleton<ISingletonService, LifetimeDemoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ── 3. Custom Middleware (Paling awal di pipeline request) ────
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
