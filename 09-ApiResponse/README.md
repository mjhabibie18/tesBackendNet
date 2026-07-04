# 📦 09 — API Response

## Standarisasi Response

Response API yang konsisten membuat:
- Client lebih mudah handle response
- Debugging lebih mudah
- Professional dan production-ready

---

## Format Response yang Disarankan

```json
// SUCCESS
{
  "success": true,
  "message": "Berhasil",
  "data": { ... },
  "errors": null,
  "timestamp": "2024-01-01T00:00:00Z"
}

// SINGLE RESOURCE
{
  "success": true,
  "data": { "id": 1, "name": "Laptop" }
}

// PAGINATED LIST
{
  "success": true,
  "data": {
    "items": [...],
    "totalCount": 100,
    "currentPage": 1,
    "pageSize": 10,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}

// ERROR
{
  "success": false,
  "message": "Validasi gagal",
  "errors": ["Nama wajib diisi", "Harga tidak boleh negatif"],
  "timestamp": "2024-01-01T00:00:00Z"
}
```

---

## Implementation

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "Berhasil")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<object?> Fail(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
```

---

## 🏋️ Latihan

- [ ] Implementasi ApiResponse generic
- [ ] Tambahkan metadata (request ID, version)
- [ ] Buat extension method untuk common responses
