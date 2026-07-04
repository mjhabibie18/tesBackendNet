// ============================================================
// ProductServiceClient.cs — Client Komunikasi Antar Service (Synchronous)
// ============================================================
// Client ini mendemonstrasikan pemanggilan HTTP sinkron
// dari Order Service ke Product Service menggunakan HttpClient,
// lengkap dengan ketahanan jaringan (Resilience Retry Policy).
// ============================================================

using System.Net.Http.Json;

namespace TesBackendNet.MicroservicesDemo.Services;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class ProductServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(HttpClient httpClient, ILogger<ProductServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductDetailsAsync(int productId)
    {
        _logger.LogInformation("[HTTP Call] Memulai pemanggilan ke Product Service untuk ID: {Id}", productId);

        try
        {
            // Panggil API Product Service secara internal (menggunakan base address yang terdaftar)
            var response = await _httpClient.GetAsync($"api/products/{productId}");

            if (response.IsSuccessStatusCode)
            {
                var product = await response.Content.ReadFromJsonAsync<ProductDto>();
                return product;
            }

            _logger.LogWarning("[HTTP Call] Product Service mengembalikan status: {Status}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[HTTP Call] Gagal terhubung ke Product Service.");
            throw;
        }
    }
}
