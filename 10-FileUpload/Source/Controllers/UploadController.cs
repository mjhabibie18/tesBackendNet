// ============================================================
// UploadController.cs — Controller File Upload
// ============================================================
// Controller ini menangani upload file tunggal dan banyak
// dengan validasi ukuran file, tipe ekstensi, dan pencegahan
// celah keamanan Path Traversal dengan me-rename file.
// ============================================================

using Microsoft.AspNetCore.Mvc;

namespace TesBackendNet.FileUpload.Controllers;

[ApiController]
[Route("api/files")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    
    // Tipe file gambar yang diizinkan
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    
    // Tipe MIME yang diizinkan (Lebih aman dibanding hanya mengecek ekstensi)
    private static readonly string[] AllowedMimeTypes = { "image/jpeg", "image/png", "image/gif" };

    // Maksimal ukuran file: 5 MB
    private const long MaxFileSizeBytes = 5 * 1024 * 1024;

    public UploadController(IWebHostEnvironment env)
    {
        _env = env;
    }

    // ================================================================
    // POST /api/files/upload
    // ================================================================
    
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
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { Success = false, Message = "Ekstensi file tidak diizinkan. Hanya menerima .jpg, .jpeg, .png, .gif" });

        // ── 4. Validasi MIME Type ───────────────────────────────────
        if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            return BadRequest(new { Success = false, Message = "MIME Type file tidak valid." });

        // ── 5. Rename File (Menghindari Path Traversal & Duplikasi) ──
        // Menggunakan GUID agar nama file selalu unik
        var newFileName = $"{Guid.NewGuid()}{ext}";
        
        // Folder tujuan penyimpanan di server: wwwroot/uploads
        var uploadDirectory = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadDirectory))
            Directory.CreateDirectory(uploadDirectory);

        var destinationPath = Path.Combine(uploadDirectory, newFileName);

        // ── 6. Simpan File Ke Disk ──────────────────────────────────
        using (var stream = new FileStream(destinationPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Return detail file yang berhasil disimpan beserta URL aksesnya
        return Ok(new
        {
            Success = true,
            Message = "File berhasil diunggah.",
            Data = new
            {
                OriginalName = file.FileName,
                SavedName = newFileName,
                Url = $"/uploads/{newFileName}",
                Size = file.Length
            }
        });
    }

    // ================================================================
    // POST /api/files/upload-multiple
    // ================================================================
    
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
            // Validasi per file
            if (file.Length == 0 || file.Length > MaxFileSizeBytes) continue;
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext)) continue;

            var newFileName = $"{Guid.NewGuid()}{ext}";
            var destinationPath = Path.Combine(uploadDirectory, newFileName);

            using (var stream = new FileStream(destinationPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            resultList.Add(new
            {
                OriginalName = file.FileName,
                SavedName = newFileName,
                Url = $"/uploads/{newFileName}"
            });
        }

        return Ok(new
        {
            Success = true,
            Message = $"{resultList.Count} file berhasil diunggah.",
            Data = resultList
        });
    }
}
