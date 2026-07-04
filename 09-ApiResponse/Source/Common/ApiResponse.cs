// ============================================================
// ApiResponse.cs — Standar Response API Wrapper dengan Metadata
// ============================================================
// Menampung metadata tambahan seperti RequestId (untuk tracing)
// dan Version (untuk konsistensi release API).
// ============================================================

namespace TesBackendNet.ApiResponse.Common;

/// <summary>Wrapper Response API utama.</summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // ── Metadata Tracing & Audit ──────────────────────────────
    public string? RequestId { get; set; }
    public string ApiVersion { get; set; } = "1.0";
}

/// <summary>Wrapper khusus untuk pagination response.</summary>
public class PagedData<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;

    public PagedData(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }
}

/// <summary>Static factory helper class.</summary>
public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "Berhasil", string? requestId = null)
        => new()
        {
            Success = true,
            Message = message,
            Data = data,
            RequestId = requestId
        };

    public static ApiResponse<object?> Fail(string message, List<string>? errors = null, string? requestId = null)
        => new()
        {
            Success = false,
            Message = message,
            Errors = errors,
            RequestId = requestId
        };
}
