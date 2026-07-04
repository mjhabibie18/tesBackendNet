// ============================================================
// RequestLoggingMiddleware.cs — Custom HTTP Middleware
// ============================================================
// Middleware beroperasi di level HTTP Request Pipeline.
// Kita mencatat method HTTP, path URL, dan durasi eksekusi request.
// ============================================================

using System.Diagnostics;

namespace TesBackendNet.Framework.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("[Middleware LOG] Incoming Request: {Method} {Path}", 
            context.Request.Method, context.Request.Path);

        // Teruskan ke middleware berikutnya dalam pipeline
        await _next(context);

        stopwatch.Stop();
        
        _logger.LogInformation("[Middleware LOG] Outgoing Response: {StatusCode} (Processed in {ElapsedMs}ms)", 
            context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
