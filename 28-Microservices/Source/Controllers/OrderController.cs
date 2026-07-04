// ============================================================
// OrderController.cs — Controller Microservice Simulasi
// ============================================================

using Microsoft.AspNetCore.Mvc;
using TesBackendNet.MicroservicesDemo.Services;

namespace TesBackendNet.MicroservicesDemo.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly ProductServiceClient _productClient;
    private readonly ILogger<OrderController> _logger;

    public OrderController(ProductServiceClient productClient, ILogger<OrderController> logger)
    {
        _productClient = productClient;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromQuery] int productId, [FromQuery] int quantity)
    {
        _logger.LogInformation("[Order Service] Menerima request pemesanan. Produk ID: {ProductId}, Qty: {Qty}", productId, quantity);

        // 1. Hubungi Product Service untuk validasi info produk (Sync HTTP Call)
        ProductDto? product;
        try
        {
            product = await _productClient.GetProductDetailsAsync(productId);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                Success = false,
                Message = "Product Service sedang tidak tersedia. Silakan coba beberapa saat lagi (Circuit Breaker/Resilience)."
            });
        }

        if (product == null)
        {
            return BadRequest(new { Success = false, Message = $"Produk dengan ID {productId} tidak ditemukan di Product Catalog." });
        }

        // 2. Simulasi pembuatan data pesanan
        var orderId = new Random().Next(1000, 9999);
        var totalAmount = product.Price * quantity;

        _logger.LogInformation("[Order Service] Order #{OrderId} berhasil dibuat. Total: {Total}", orderId, totalAmount);

        // 3. Simulasi Asynchronous Event-Driven Pub/Sub
        // Di arsitektur microservice, kita mempublikasikan event 'OrderCreatedEvent' ke Message Broker (RabbitMQ)
        // secara asinkron tanpa memblokir response HTTP client.
        PublishToMessageBrokerDummy(orderId, product.Name, quantity);

        return Created("", new
        {
            Success = true,
            OrderId = orderId,
            ProductName = product.Name,
            Quantity = quantity,
            Total = totalAmount,
            Message = "Pesanan berhasil dibuat. Event notifikasi dipublikasikan ke Queue Broker."
        });
    }

    private void PublishToMessageBrokerDummy(int orderId, string productName, int qty)
    {
        // Simulasi pengiriman event asinkron
        _logger.LogInformation("[Event Broker] PUBLISH: OrderCreatedEvent -> OrderId: {OrderId}, Item: {Item}, Qty: {Qty}", 
            orderId, productName, qty);
    }
}

// ── Dummy Controller Meniru Product Service di Port yang Sama ──
[ApiController]
[Route("api/products")]
public class DummyProductServiceController : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult GetProduct(int id)
    {
        if (id == 1)
        {
            return Ok(new { Id = 1, Name = "Smartphone Android", Price = 3500000 });
        }
        if (id == 2)
        {
            return Ok(new { Id = 2, Name = "Laptop Slim 14 Inch", Price = 9800000 });
        }

        return NotFound();
    }
}
