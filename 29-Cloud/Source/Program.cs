// ============================================================
// Program.cs — Demo Implementasi Azure SDK & Cloud Services
// ============================================================

using Azure.Identity;
using Azure.Storage.Blobs;

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("☁️ DEMO INTEGRASI AZURE CLOUD SERVICES (BLOB STORAGE & KEY VAULT)");
Console.WriteLine("============================================================\n");

// ── 1. Azure Key Vault Demo (Abstraksi Konsep) ────────────────
Console.WriteLine("--- 1. AZURE KEY VAULT (SECRETS) ---");
Console.WriteLine("Di Azure, kita menghindari penyimpanan connection string sensitif di appsettings.json.");
Console.WriteLine("Sebagai gantinya, kita gunakan Azure Identity (DefaultAzureCredential) untuk mengambil");
Console.WriteLine("secret secara aman dari Key Vault pada saat startup:\n");

Console.WriteLine("```csharp");
Console.WriteLine("// Setup Key Vault di Program.cs");
Console.WriteLine("var keyVaultUri = new Uri(\"https://tesbackendnet-vault.vault.azure.net/\");");
Console.WriteLine("builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());");
Console.WriteLine("```");
Console.WriteLine("Status: Kode siap digunakan pada environment production Azure.\n");

// ── 2. Azure Blob Storage File Upload Demo ────────────────────
Console.WriteLine("--- 2. AZURE BLOB STORAGE (FILE UPLOAD) ---");

// Connection string Azure Blob Storage Emulator lokal (AzuriLite) atau Azure Asli
string azureConnectionString = "UseDevelopmentStorage=true"; // Default local development emulator

Console.WriteLine($"Menghubungkan ke Storage Account via Connection String: '{azureConnectionString}'...");

try
{
    // Inisialisasi client penampung container blob
    var blobServiceClient = new BlobServiceClient(azureConnectionString);
    var containerClient = blobServiceClient.GetBlobContainerClient("user-uploads");

    Console.WriteLine("Memeriksa / membuat container 'user-uploads'...");
    // Di Azure emulator / local development, baris ini akan sukses jika emulator berjalan.
    // Kita tangkap exception dengan aman jika emulator belum dinyalakan agar program tidak crash.
    await containerClient.CreateIfNotExistsAsync();

    Console.WriteLine("Menyiapkan dummy file upload stream...");
    string fileName = $"cv-developer-{Guid.NewGuid()}.pdf";
    var fileContent = "DUMMY PDF CONTENT FOR RESUME";
    using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(fileContent));

    var blobClient = containerClient.GetBlobClient(fileName);
    
    Console.WriteLine($"Mengunggah file '{fileName}' ke Azure Blob Storage...");
    await blobClient.UploadAsync(stream, overwrite: true);

    Console.WriteLine("\n[SUKSES] File berhasil terunggah ke Cloud Blob Storage!");
    Console.WriteLine($"URL Public Akses: {blobClient.Uri}\n");
}
catch (Exception ex)
{
    Console.WriteLine("\n[INFO] Azure Storage Emulator tidak aktif di lokal.");
    Console.WriteLine("Detail pesan error yang ditangkap secara aman:");
    Console.WriteLine($"> {ex.Message}");
    Console.WriteLine("\n[SIMULASI FALLBACK] Melakukan fallback upload ke Local File System...");
    
    // Fallback: Tulis ke file system lokal untuk menunjukkan behavior aman
    string localUploadPath = Path.Combine(AppContext.BaseDirectory, "uploads");
    Directory.CreateDirectory(localUploadPath);
    
    string localFileName = $"fallback-cv-{Guid.NewGuid()}.pdf";
    string fullPath = Path.Combine(localUploadPath, localFileName);
    
    await File.WriteAllTextAsync(fullPath, "DUMMY LOCAL FALLBACK CONTENT");
    
    Console.WriteLine($"[FALLBACK SUKSES] File berhasil disimpan secara lokal di:");
    Console.WriteLine($"> {fullPath}");
}

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Cloud Services Selesai!");
Console.WriteLine("============================================================");
