// ============================================================
// Nama File: ApiResponse.cs — Standar Response Wrapper dengan Metadata
// Folder: 09-ApiResponse/Source/Common/
// ============================================================
// 1. PENJELASAN FOLDER (ApiResponse/Common):
//    - Tujuan: Mendefinisikan format respon JSON yang seragam (unified response envelope) untuk seluruh endpoint API.
//    - Kapan Digunakan: Selalu digunakan di setiap endpoint agar klien mendapatkan struktur data yang konsisten dan mudah diparsing.
//    - Hubungan: ApiResponse<T> digunakan oleh semua Controller, termasuk modul 01-CRUD, 04-RESTfulAPI, dll.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan kelas generik (ApiResponse<T>) dan factory helper (ApiResponse static)
//      untuk membungkus semua payload respon API.
//    - Mengapa Diperlukan: Tanpa struktur respons yang seragam, klien (frontend/mobile) harus menangani format respons 
//      yang berbeda-beda untuk setiap endpoint. Ini sangat rentan menimbulkan bug di sisi klien.
//    - Jika Dihapus: Setiap endpoint harus membuat struktur responsnya sendiri secara ad-hoc, melanggar prinsip DRY.
// ============================================================

namespace TesBackendNet.ApiResponse.Common;

/// <summary>
/// TUJUAN CLASS:
/// Wrapper generik untuk membungkus SEMUA respons API dalam satu format amplop (envelope) yang konsisten.
/// 
/// MENGAPA MENGGUNAKAN GENERICS (ApiResponse<T>)?
/// Tipe data generik T memungkinkan kelas ini digunakan kembali untuk membungkus tipe data apa saja:
/// - ApiResponse<ProductDto>: untuk mengembalikan detail satu produk.
/// - ApiResponse<List<UserDto>>: untuk mengembalikan daftar pengguna.
/// - ApiResponse<PagedData<OrderDto>>: untuk mengembalikan daftar terpaginasi.
/// Tanpa generics, kita harus membuat kelas wrapper terpisah untuk setiap tipe data.
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// FUNGSI PROPERTY: Indikator keberhasilan operasi.
    /// ALASAN: Klien cukup memeriksa `success: true/false` sebelum memproses data.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Pesan informatif yang menjelaskan hasil operasi.
    /// ALASAN: Memberikan konteks yang mudah dibaca manusia (human-readable) tentang apa yang terjadi.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Payload utama yang dikembalikan kepada klien.
    /// ALASAN NULLABLE (T?): Pada respons error (Success = false), properti Data tidak relevan dan dibiarkan null.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Daftar pesan error yang spesifik (opsional).
    /// KAPAN DIGUNAKAN: Saat validasi input gagal, daftar error per field dapat disisipkan di sini.
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Waktu server saat respons dibuat (UTC).
    /// ALASAN: Membantu debugging ketika klien melaporkan bug — developer dapat mencocokkan timestamp respons dengan log server.
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// FUNGSI PROPERTY: ID unik dari request HTTP untuk keperluan distributed tracing dan pelacakan log.
    /// KAPAN DIGUNAKAN: Saat bug dilaporkan, ID ini dapat digunakan untuk mencari log yang tepat di sistem log terpusat (Kibana, Splunk, dll.).
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Versi API saat ini.
    /// ALASAN: Membantu klien mengetahui versi API yang merespons, berguna saat mengelola breaking changes.
    /// </summary>
    public string ApiVersion { get; set; } = "1.0";
}

/// <summary>
/// TUJUAN CLASS:
/// Wrapper paginasi yang membungkus data koleksi beserta metadata navigasi halaman.
/// 
/// ALASAN MENGGUNAKAN COMPUTED PROPERTIES (TotalPages, HasNextPage, HasPreviousPage):
/// Computed properties dihitung secara otomatis dari TotalCount dan PageSize tanpa perlu data tambahan dari database.
/// Ini menghemat round-trip database dan membebaskan klien dari keharusan menghitung paginasi sendiri.
/// </summary>
public class PagedData<T>
{
    /// <summary>Koleksi item data pada halaman saat ini.</summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>Total seluruh record di database (tanpa paginasi).</summary>
    public int TotalCount { get; set; }

    /// <summary>Nomor halaman saat ini (dimulai dari 1).</summary>
    public int CurrentPage { get; set; }

    /// <summary>Jumlah item per halaman.</summary>
    public int PageSize { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Menghitung total halaman secara otomatis.
    /// FORMULA: Math.Ceiling(TotalCount / PageSize) memastikan sisa item pada pembagian tidak bulat tetap masuk ke halaman terakhir.
    /// Contoh: 25 item / 10 per halaman = Ceiling(2.5) = 3 halaman.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>Indikator apakah halaman berikutnya tersedia.</summary>
    public bool HasNextPage => CurrentPage < TotalPages;

    /// <summary>Indikator apakah halaman sebelumnya tersedia.</summary>
    public bool HasPreviousPage => CurrentPage > 1;

    public PagedData(IEnumerable<T> items, int totalCount, int currentPage, int pageSize)
    {
        Items       = items;
        TotalCount  = totalCount;
        CurrentPage = currentPage;
        PageSize    = pageSize;
    }
}

/// <summary>
/// TUJUAN CLASS:
/// Static factory helper yang menyederhanakan pembuatan objek ApiResponse<T>.
/// 
/// POLA DESAIN: Factory Method.
/// Daripada membuat instance `new ApiResponse<T>() { ... }` di setiap endpoint,
/// cukup panggil `ApiResponse.Success(data, "Pesan")` — lebih ringkas dan konsisten.
/// </summary>
public static class ApiResponse
{
    /// <summary>
    /// FUNGSI METHOD: Membuat respons sukses.
    /// PARAMETER:
    ///  - data: Payload yang akan dikembalikan ke klien (bertipe generik T).
    ///  - message: Pesan informatif.
    ///  - requestId: ID unik request untuk tracing.
    /// NILAI KEMBALIAN: ApiResponse<T> dengan Success = true.
    /// </summary>
    public static ApiResponse<T> Success<T>(T data, string message = "Berhasil", string? requestId = null)
        => new()
        {
            Success   = true,
            Message   = message,
            Data      = data,
            RequestId = requestId
        };

    /// <summary>
    /// FUNGSI METHOD: Membuat respons gagal.
    /// PARAMETER:
    ///  - message: Pesan error utama.
    ///  - errors: Daftar error validasi per field (opsional).
    ///  - requestId: ID unik request untuk tracing.
    /// NILAI KEMBALIAN: ApiResponse<object?> dengan Success = false dan Data = null.
    /// </summary>
    public static ApiResponse<object?> Fail(string message, List<string>? errors = null, string? requestId = null)
        => new()
        {
            Success   = false,
            Message   = message,
            Errors    = errors,
            RequestId = requestId
        };
}
