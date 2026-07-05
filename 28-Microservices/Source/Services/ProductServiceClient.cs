// ============================================================
// Nama File: ProductServiceClient.cs — Client Komunikasi Antar Microservice (Synchronous HTTP)
// Folder: 28-Microservices/Source/Services/
// ============================================================
// 1. PENJELASAN FOLDER (Microservices):
//    - Tujuan: Mendemonstrasikan pola komunikasi antar-service dalam arsitektur microservices.
//    - Kapan Digunakan: Saat arsitektur sistem dipecah menjadi banyak service kecil yang berjalan mandiri
//      dan perlu saling berkomunikasi untuk menyelesaikan satu proses bisnis.
//    - Hubungan: Order Service (client) memanggil Product Service (server) untuk mendapatkan detail produk.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengenkapsulasi detail komunikasi HTTP dari Order Service ke Product Service
//      dalam satu kelas client yang bersih dengan retry policy berbasis Polly.
//    - Mengapa Diperlukan: Tanpa kelas client terdedikasi, logika HTTP call tersebar di seluruh controller
//      dan sulit dikelola, diuji, atau diubah.
//    - Hubungan File: Dikonfigurasi di Program.cs menggunakan `AddHttpClient<ProductServiceClient>()`.
//    - Jika Dihapus: Order Service tidak dapat mengambil informasi produk dari Product Service.
// ============================================================

using System.Net.Http.Json;

namespace TesBackendNet.MicroservicesDemo.Services;

/// <summary>
/// TUJUAN CLASS:
/// Data Transfer Object (DTO) yang mewakili struktur respons dari Product Service API.
/// Order Service tidak perlu mengetahui skema database Product Service — hanya DTO yang diekspos.
/// </summary>
public class ProductDto
{
    public int     Id    { get; set; }
    public string  Name  { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

/// <summary>
/// TUJUAN CLASS:
/// Client yang mengelola semua komunikasi HTTP dari Order Service ke Product Service.
/// 
/// ALASAN MENGGUNAKAN TYPED HTTP CLIENT (IHttpClientFactory / AddHttpClient<T>):
/// Masalah klasik dengan `new HttpClient()`: setiap instansi membuat soket (socket) baru.
/// Jika banyak request terjadi bersamaan, soket menumpuk dan akhirnya terjadi Socket Exhaustion (kehabisan soket).
/// `IHttpClientFactory` mengelola pool soket secara efisien, mencegah masalah ini.
/// 
/// RETRY POLICY (Polly):
/// Jaringan antar-service rentan terhadap kegagalan sementara (transient failures): timeout, network blip.
/// Polly dikonfigurasi di Program.cs untuk mencoba ulang request yang gagal hingga 3 kali secara otomatis
/// sebelum akhirnya melempar exception. Ini meningkatkan ketahanan (resilience) sistem.
/// </summary>
public class ProductServiceClient
{
    private readonly HttpClient                      _httpClient;
    private readonly ILogger<ProductServiceClient>   _logger;

    /// <summary>
    /// CONSTRUCTOR: HttpClient disuntikkan dari pool yang dikelola IHttpClientFactory.
    /// </summary>
    public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger     = logger;
    }

    /// <summary>
    /// FUNGSI METHOD: Mengambil detail produk dari Product Service melalui HTTP GET.
    /// PARAMETER: productId (ID produk yang ingin diambil informasinya).
    /// NILAI KEMBALIAN: Task<ProductDto?> — nullable; null jika produk tidak ditemukan atau service tidak tersedia.
    /// 
    /// ALUR EKSEKUSI:
    /// 1. `_httpClient.GetAsync(...)`: Mengirim HTTP GET request ke Product Service.
    ///    Base URL sudah dikonfigurasi di Program.cs, sehingga hanya path relatif yang perlu ditulis.
    /// 2. `response.IsSuccessStatusCode`: Memeriksa apakah status code berada di range 200-299 (sukses).
    /// 3. `response.Content.ReadFromJsonAsync<ProductDto>()`: Deserialasi JSON body response ke objek ProductDto.
    ///    Method ini adalah shortcut dari System.Net.Http.Json (tidak perlu JsonSerializer manual).
    /// 4. Blok `catch`: Mencatat exception dan melemparnya kembali agar caller dapat menanganinya lebih lanjut.
    ///    (`throw;` tanpa argumen — melempar ulang exception asli dengan stack trace yang terjaga).
    /// 
    /// CATATAN RESILIENCE:
    /// Jika Product Service mati atau timeout, Polly retry policy akan mencoba ulang hingga 3 kali
    /// sebelum akhirnya exception ini di-throw ke Order Controller.
    /// </summary>
    public async Task<ProductDto?> GetProductDetailsAsync(int productId)
    {
        _logger.LogInformation("[HTTP Call] Memulai pemanggilan ke Product Service untuk ID: {Id}", productId);

        try
        {
            // Kirim GET request ke Product Service
            var response = await _httpClient.GetAsync($"api/products/{productId}");

            if (response.IsSuccessStatusCode)
            {
                // Deserialasi JSON body → objek ProductDto secara otomatis
                var product = await response.Content.ReadFromJsonAsync<ProductDto>();
                return product;
            }

            // Log jika Product Service mengembalikan status non-2xx (misal: 404)
            _logger.LogWarning("[HTTP Call] Product Service mengembalikan status: {Status}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            // Catat exception dan lempar ulang — biarkan caller memutuskan cara menanganinya
            _logger.LogError(ex, "[HTTP Call] Gagal terhubung ke Product Service.");
            throw;
        }
    }
}
