// ============================================================
// Nama File: UploadController.cs — Controller File Upload
// Folder: 10-FileUpload/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (FileUpload):
//    - Tujuan: Menangani proses penerimaan file dari klien, memvalidasinya dari berbagai sisi keamanan,
//      dan menyimpannya secara aman ke sistem berkas (file system) server.
//    - Kapan Digunakan: Saat fitur avatar pengguna, lampiran dokumen, atau galeri foto diperlukan.
//    - Hubungan: Diakses oleh klien melalui form multipart/form-data. Hasilnya dapat disimpan di
//      database (nama file) dan diakses via URL statis (wwwroot/uploads).
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan 2 endpoint upload: satu file tunggal dan banyak file sekaligus.
//    - Mengapa Diperlukan: Upload file tanpa validasi dan pengamanan nama file adalah celah keamanan serius
//      (Path Traversal Attack, Malware Upload, DDoS via file besar).
//    - Hubungan File: Menggunakan IWebHostEnvironment untuk mendapatkan path fisik folder wwwroot.
//    - Jika Dihapus: Fitur upload file pada sistem tidak berfungsi.
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.FileUpload.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang mengimplementasikan file upload yang aman dengan validasi berlapis.
/// 
/// VALIDASI BERLAPIS YANG DITERAPKAN:
/// 1. Validasi keberadaan file (null/empty check).
/// 2. Validasi ukuran file maksimal (mencegah DDoS melalui upload file besar).
/// 3. Validasi ekstensi file (whitelist ekstensi yang diizinkan).
/// 4. Validasi MIME type (validasi lebih dalam daripada sekadar ekstensi).
/// 5. Rename file dengan GUID (mencegah Path Traversal Attack dan duplikasi nama).
/// </summary>
[ApiController]
[Route("api/files")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    
    /// <summary>
    /// Daftar putih ekstensi file yang diizinkan. 
    /// KEAMANAN: Hanya terima format gambar. Jangan izinkan .exe, .php, .aspx, dll.
    /// </summary>
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    
    /// <summary>
    /// Daftar MIME type yang diizinkan.
    /// MENGAPA LEBIH AMAN DARI EKSTENSI SAJA:
    /// Penyerang dapat mengganti ekstensi file (misal: malware.php → malware.jpg). Tetapi isi file (magic bytes)
    /// tetap menunjukkan tipe aslinya melalui Content-Type header yang lebih sulit dipalsukan secara konsisten.
    /// </summary>
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/gif" };

    /// <summary>
    /// Batas ukuran file maksimal: 5 MB.
    /// PERHITUNGAN: 5 * 1024 (KB) * 1024 (MB) = 5,242,880 bytes.
    /// </summary>
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan IWebHostEnvironment untuk mendapatkan path fisik folder wwwroot.
    /// IWebHostEnvironment menyediakan informasi direktori kerja aplikasi web saat runtime.
    /// </summary>
    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    /// <summary>
    /// FUNGSI METHOD: Menerima dan menyimpan satu file gambar dari klien.
    /// PARAMETER: file (IFormFile) — representasi file yang dikirim melalui HTTP multipart/form-data.
    /// NILAI KEMBALIAN: Task<IActionResult> — HTTP 200 OK berisi URL akses file, atau HTTP 400 jika validasi gagal.
    /// 
    /// ALUR VALIDASI KEAMANAN (Berlapis):
    /// Setiap langkah validasi gagal langsung mengembalikan 400 Bad Request tanpa melanjutkan ke langkah berikutnya (Fail-Fast).
    /// 
    /// BARIS KODE PENTING:
    /// - `[Consumes("multipart/form-data")]`: Memberi tahu Swagger/OpenAPI bahwa endpoint ini menerima form upload, 
    ///   bukan JSON. Tanpa ini, Swagger tidak akan menampilkan input file di UI.
    /// - `IFormFile`: Interface bawaan ASP.NET Core yang membungkus file dari form upload. Menyediakan metadata 
    ///   seperti FileName, ContentType, Length, dan method CopyToAsync().
    /// - `Guid.NewGuid()`: Menghasilkan identifier unik universal (UUID v4, 128-bit) secara acak sebagai nama file baru.
    ///   Ini mencegah: (1) duplikasi nama file, (2) Path Traversal Attack dari nama file seperti `../../evil.sh`.
    /// - `using (var stream = ...)`: Memastikan FileStream dibuang (disposed) secara otomatis setelah blok selesai,
    ///   melepaskan lock pada file system meskipun terjadi exception.
    /// - `await file.CopyToAsync(stream)`: Menyalin konten file secara asinkron ke disk tanpa memblokir thread server.
    /// </summary>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadSingle(IFormFile file)
    {
        // ── 1. Validasi Keberadaan File ─────────────────────────────
        if (file == null || file.Length == 0)
            return BadRequest(new { Success = false, Message = "File tidak boleh kosong." });

        // ── 2. Validasi Ukuran File ─────────────────────────────────
        if (file.Length > MaxFileSizeBytes)
            return BadRequest(new { Success = false, Message = "Ukuran file melebihi batas maksimal 5 MB." });

        // ── 3. Validasi Ekstensi File ───────────────────────────────
        // `ToLowerInvariant()` memastikan .JPG, .Jpg, .jpg semuanya sama
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { Success = false, Message = "Ekstensi file tidak diizinkan. Hanya menerima .jpg, .jpeg, .png, .gif" });

        // ── 4. Validasi MIME Type ───────────────────────────────────
        if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new { Success = false, Message = "MIME Type file tidak valid." });

        // ── 5. Rename File dengan GUID ──────────────────────────────
        // GUID memastikan nama file selalu unik dan mencegah Path Traversal
        var newFileName = $"{Guid.NewGuid()}{ext}";
        
        // Membangun path fisik folder tujuan penyimpanan di server
        var uploadDirectory = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadDirectory))
            Directory.CreateDirectory(uploadDirectory); // Buat folder jika belum ada

        var destinationPath = Path.Combine(uploadDirectory, newFileName);

        // ── 6. Simpan File ke Disk Secara Asinkron ──────────────────
        using (var stream = new FileStream(destinationPath, FileMode.Create))
        {
            // CopyToAsync: Menulis stream file ke disk tanpa memblokir thread
            await file.CopyToAsync(stream);
        }

        return Ok(new
        {
            Success = true,
            Message = "File berhasil diunggah.",
            Data    = new
            {
                OriginalName = file.FileName,
                SavedName    = newFileName,
                Url          = $"/uploads/{newFileName}",  // URL relatif akses publik
                Size         = file.Length
            }
        });
    }

    /// <summary>
    /// FUNGSI METHOD: Menerima dan menyimpan banyak file gambar sekaligus.
    /// PARAMETER: files (List<IFormFile>) — daftar file dari form upload.
    /// NILAI KEMBALIAN: Task<IActionResult> — HTTP 200 OK berisi daftar file yang berhasil disimpan.
    /// 
    /// PERBEDAAN DENGAN SINGLE UPLOAD:
    /// Endpoint ini memproses setiap file dalam loop. File yang tidak memenuhi validasi di-skip (continue) 
    /// tanpa menghentikan proses upload file lainnya dalam batch yang sama.
    /// </summary>
    [HttpPost("upload-multiple")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadMultiple(List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest(new { Success = false, Message = "Tidak ada file yang diunggah." });

        var uploadDirectory = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadDirectory))
            Directory.CreateDirectory(uploadDirectory);

        var resultList = new List<object>();

        foreach (var file in files)
        {
            // Validasi per file — lewati (skip) file yang tidak valid
            if (file.Length == 0 || file.Length > MaxFileSizeBytes) continue;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext)) continue;

            var newFileName     = $"{Guid.NewGuid()}{ext}";
            var destinationPath = Path.Combine(uploadDirectory, newFileName);

            using (var stream = new FileStream(destinationPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            resultList.Add(new
            {
                OriginalName = file.FileName,
                SavedName    = newFileName,
                Url          = $"/uploads/{newFileName}"
            });
        }

        return Ok(new
        {
            Success = true,
            Message = $"{resultList.Count} file berhasil diunggah.",
            Data    = resultList
        });
    }
}
