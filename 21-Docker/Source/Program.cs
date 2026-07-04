var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/", () => new
{
    Message = "Hello from ASP.NET Core inside Docker Container! 🐳",
    Timestamp = DateTime.UtcNow,
    Environment = builder.Environment.EnvironmentName
});

app.MapControllers();

app.Run();
