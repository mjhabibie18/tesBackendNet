# 📁 10 — File Upload

## Konsep File Upload

File upload di REST API menggunakan `multipart/form-data` (bukan JSON).

---

## Setup di Program.cs

```csharp
// Konfigurasi ukuran file maksimal
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 MB
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
});
```

---

## Upload Single File

```csharp
[HttpPost("upload")]
[Consumes("multipart/form-data")]
public async Task<IActionResult> Upload(IFormFile file)
{
    // Validasi
    if (file == null || file.Length == 0)
        return BadRequest("File tidak boleh kosong");

    // Validasi tipe file
    var allowedTypes = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
    if (!allowedTypes.Contains(ext))
        return BadRequest("Tipe file tidak diizinkan");

    // Validasi ukuran
    if (file.Length > 5 * 1024 * 1024) // 5 MB
        return BadRequest("File terlalu besar (max 5 MB)");

    // Simpan file
    var fileName  = $"{Guid.NewGuid()}{ext}";
    var uploadDir = Path.Combine("wwwroot", "uploads");
    Directory.CreateDirectory(uploadDir);

    var filePath  = Path.Combine(uploadDir, fileName);
    using var stream = File.Create(filePath);
    await file.CopyToAsync(stream);

    return Ok(new { fileName, url = $"/uploads/{fileName}" });
}
```

---

## Upload Multiple Files

```csharp
[HttpPost("upload-multiple")]
public async Task<IActionResult> UploadMultiple(List<IFormFile> files)
{
    var uploadedFiles = new List<string>();

    foreach (var file in files)
    {
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine("wwwroot", "uploads", fileName);
        using var stream = File.Create(path);
        await file.CopyToAsync(stream);
        uploadedFiles.Add(fileName);
    }

    return Ok(new { files = uploadedFiles });
}
```

---

## Serve Static Files

```csharp
// Program.cs
app.UseStaticFiles(); // serve dari wwwroot/

// Akses: GET /uploads/filename.jpg
```

---

## ✅ Best Practice

1. **Validasi tipe MIME** (bukan hanya ekstensi)
2. **Rename file**: jangan gunakan nama asli (path traversal attack)
3. **Store di luar wwwroot** untuk file private
4. **Batasi ukuran** file
5. **Scan virus** untuk production
6. **Gunakan cloud storage** (Azure Blob, S3) untuk production

---

## 🎤 Tips Interview

**Q: "Bagaimana cara aman simpan file upload?"**
```
1. Validate MIME type (bukan hanya ekstensi)
2. Rename dengan GUID
3. Scan antivirus
4. Simpan di cloud storage (bukan server lokal)
5. Batasi ukuran dan tipe file yang diizinkan
```
