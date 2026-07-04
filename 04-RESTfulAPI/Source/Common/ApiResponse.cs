// ============================================================
// ApiResponse.cs — Wrapper Response Standar dengan Generics
// ============================================================

namespace TesBackendNet.RESTfulAPI.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "Berhasil")
        => new()
        {
            Success   = true,
            Message   = message,
            Data      = data
        };

    public static ApiResponse<object?> Fail(string message)
        => new()
        {
            Success   = false,
            Message   = message,
            Data      = null
        };
}
