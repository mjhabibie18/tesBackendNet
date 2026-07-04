// ============================================================
// ApiResponse.cs — Wrapper Response Standar
// ============================================================
// ApiResponse adalah class wrapper untuk standarisasi response API.
//
// Mengapa perlu wrapper?
//   - Konsistensi: semua endpoint punya format response yang sama
//   - Informasi lebih: bisa tambahkan metadata (pesan, status, error)
//   - Client lebih mudah: client tahu selalu ada field "success", "data", "message"
//   - Maintainable: jika perlu ubah format, cukup ubah di satu tempat
//
// Format response yang konsisten:
// {
//   "success": true,
//   "message": "Berhasil",
//   "data": { ... },
//   "errors": null
// }
// ============================================================

namespace TesBackendNet.CRUD.Common;

/// <summary>
/// Generic wrapper untuk semua API response.
/// T = tipe data yang dikembalikan (bisa Product, List, dll)
/// </summary>
public class ApiResponse<T>
{
    /// <summary>Apakah request berhasil?</summary>
    public bool Success { get; set; }

    /// <summary>Pesan human-readable untuk client/developer</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Data yang dikembalikan. Null jika error atau tidak ada data.</summary>
    public T? Data { get; set; }

    /// <summary>List error jika validasi gagal</summary>
    public List<string>? Errors { get; set; }

    /// <summary>Timestamp response untuk debugging</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Static factory class untuk membuat ApiResponse dengan mudah.
/// Tanpa generic parameter, untuk kemudahan penggunaan.
/// </summary>
public static class ApiResponse
{
    /// <summary>
    /// Membuat response sukses dengan data.
    /// Digunakan untuk GET, POST, PUT yang berhasil.
    /// </summary>
    public static ApiResponse<T> Success<T>(T data, string message = "Berhasil")
        => new()
        {
            Success   = true,
            Message   = message,
            Data      = data,
            Errors    = null,
            Timestamp = DateTime.UtcNow
        };

    /// <summary>
    /// Membuat response sukses tanpa data (untuk DELETE).
    /// </summary>
    public static ApiResponse<object?> Success(string message = "Berhasil")
        => new()
        {
            Success   = true,
            Message   = message,
            Data      = null,
            Errors    = null,
            Timestamp = DateTime.UtcNow
        };

    /// <summary>
    /// Membuat response gagal dengan pesan error.
    /// </summary>
    public static ApiResponse<T> Fail<T>(string message, List<string>? errors = null)
        => new()
        {
            Success   = false,
            Message   = message,
            Data      = default,
            Errors    = errors,
            Timestamp = DateTime.UtcNow
        };

    /// <summary>
    /// Membuat response gagal tanpa generic (untuk kasus umum).
    /// </summary>
    public static ApiResponse<object?> Fail(string message, List<string>? errors = null)
        => Fail<object?>(message, errors);
}

// ============================================================
// PagedResult.cs — Wrapper untuk Pagination
// ============================================================

/// <summary>
/// Wrapper untuk response yang menggunakan pagination.
/// Berisi data + metadata pagination.
/// </summary>
/// <typeparam name="T">Tipe data item dalam list</typeparam>
public class PagedResult<T>
{
    /// <summary>List data untuk halaman ini</summary>
    public List<T> Data { get; set; } = new();

    /// <summary>Total semua data (semua halaman)</summary>
    public int TotalCount { get; set; }

    /// <summary>Halaman saat ini</summary>
    public int CurrentPage { get; set; }

    /// <summary>Jumlah data per halaman</summary>
    public int PageSize { get; set; }

    // ── Computed Properties ───────────────────────────────────
    // Computed properties: dihitung otomatis dari property lain
    // Tidak disimpan di database

    /// <summary>Total halaman yang tersedia</summary>
    public int TotalPages => PageSize > 0
        ? (int)Math.Ceiling((double)TotalCount / PageSize)
        : 0;

    /// <summary>Apakah ada halaman berikutnya?</summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>Apakah ada halaman sebelumnya?</summary>
    public bool HasPreviousPage => CurrentPage > 1;
}
