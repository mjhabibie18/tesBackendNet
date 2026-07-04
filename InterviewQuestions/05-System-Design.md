# 🏗️ 05 — System Design Interview Questions

---

## Soal 1: Design URL Shortener (seperti bit.ly)

**Tingkat**: Medium | **Topik**: System Design

### Requirements
- Buat URL pendek dari URL panjang
- Redirect URL pendek ke URL asli
- Analitik: berapa kali diklik
- 100 juta URL pendek, 10 juta request/hari

### Solusi

```csharp
// Model
public class ShortUrl
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;    // "abc123"
    public string OriginalUrl { get; set; } = string.Empty;
    public int ClickCount { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
}

// Service
public class UrlShortenerService
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private readonly IMemoryCache _cache;
    private readonly AppDbContext _db;

    // Generate 6-char code
    private string GenerateCode()
    {
        var random = new Random();
        return new string(Enumerable.Range(0, 6)
            .Select(_ => Alphabet[random.Next(Alphabet.Length)])
            .ToArray());
    }

    public async Task<string> ShortenAsync(string url)
    {
        string code;
        do
        {
            code = GenerateCode();
        } while (await _db.ShortUrls.AnyAsync(u => u.Code == code)); // Ensure unique

        var shortUrl = new ShortUrl { Code = code, OriginalUrl = url };
        _db.ShortUrls.Add(shortUrl);
        await _db.SaveChangesAsync();

        // Cache untuk redirect cepat
        _cache.Set(code, url, TimeSpan.FromHours(1));

        return $"https://short.ly/{code}";
    }

    public async Task<string?> GetOriginalUrlAsync(string code)
    {
        // Check cache dulu
        if (_cache.TryGetValue(code, out string? cached))
        {
            await IncrementClickAsync(code);
            return cached;
        }

        var shortUrl = await _db.ShortUrls.FirstOrDefaultAsync(u => u.Code == code);
        if (shortUrl == null) return null;

        _cache.Set(code, shortUrl.OriginalUrl, TimeSpan.FromHours(1));
        shortUrl.ClickCount++;
        await _db.SaveChangesAsync();

        return shortUrl.OriginalUrl;
    }

    private async Task IncrementClickAsync(string code)
    {
        // Background: tidak perlu block response
        await _db.ShortUrls
            .Where(u => u.Code == code)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.ClickCount, u => u.ClickCount + 1));
    }
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class UrlController : ControllerBase
{
    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenDto dto)
    {
        var shortUrl = await _service.ShortenAsync(dto.Url);
        return Ok(new { shortUrl });
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Redirect(string code)
    {
        var originalUrl = await _service.GetOriginalUrlAsync(code);
        if (originalUrl == null) return NotFound();

        return Redirect(originalUrl); // 302 redirect
    }
}
```

### Architecture

```
Client → API Gateway → URL Service → Redis Cache → SQL Database
                                   ↘ (miss) → Database → Cache
```

---

## Soal 2: Design E-commerce Order System

**Tingkat**: Hard | **Topik**: System Design, Architecture

### Requirements
- User bisa buat order
- Cek stok sebelum order
- Payment integration
- Email notifikasi setelah order

### Solusi

```csharp
// Database Models
public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Shipped, Completed, Cancelled
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Harga saat order, bukan harga sekarang!
}

// Service dengan Transaction
public class OrderService
{
    public async Task<Order> CreateOrderAsync(int userId, CreateOrderDto dto)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        try
        {
            // 1. Validasi dan lock stok (pessimistic locking)
            var products = await _db.Products
                .Where(p => dto.Items.Select(i => i.ProductId).Contains(p.Id))
                .ToListAsync();

            foreach (var item in dto.Items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Stok {product.Name} tidak cukup");

                product.Stock -= item.Quantity; // Kurangi stok
            }

            // 2. Buat order
            var order = new Order
            {
                UserId      = userId,
                Status      = "Pending",
                TotalAmount = dto.Items.Sum(i =>
                    products.First(p => p.Id == i.ProductId).Price * i.Quantity),
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity  = i.Quantity,
                    UnitPrice = products.First(p => p.Id == i.ProductId).Price
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            // 3. Background: kirim email (tidak block response)
            _ = Task.Run(() => _emailService.SendOrderConfirmationAsync(order.Id));

            return order;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
```

---

## Soal 3: Design Rate Limiting

**Tingkat**: Medium | **Topik**: System Design, Security

### Solusi

```csharp
// Token Bucket Algorithm
public class RateLimiter
{
    private readonly IDistributedCache _cache;

    // Max 60 request per menit per IP
    private const int MaxRequests    = 60;
    private const int WindowSeconds  = 60;

    public async Task<bool> IsAllowedAsync(string key)
    {
        var cacheKey  = $"ratelimit:{key}";
        var countStr  = await _cache.GetStringAsync(cacheKey);
        var count     = countStr != null ? int.Parse(countStr) : 0;

        if (count >= MaxRequests)
            return false; // Rate limit exceeded

        await _cache.SetStringAsync(cacheKey, (count + 1).ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(WindowSeconds)
            });

        return true;
    }
}

// Middleware
public class RateLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RateLimiter _limiter;

    public async Task InvokeAsync(HttpContext context)
    {
        var key = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (!await _limiter.IsAllowedAsync(key))
        {
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded. Try again later."
            });
            return;
        }

        await _next(context);
    }
}
```

---

## Soal 4–15: (Ringkasan)

**4.** Design Notification System
> Publisher-Subscriber pattern: event → queue (RabbitMQ) → consumer (email, SMS, push)

**5.** Design Search dengan Elasticsearch
> Full-text search: index documents → search → highlight → pagination

**6.** Design File Storage System
> Upload → Virus scan → Resize (image) → Store (Blob) → CDN → Serve

**7.** Design Authentication System (OAuth2)
> Authorization Server + Resource Server, Access Token + Refresh Token flow

**8.** Design Audit Log System
> Middleware: log setiap request + response → queue → write to immutable storage

**9.** Design Real-time Chat (SignalR)
> WebSocket hub → group management → message broadcasting → persistence

**10.** Design Product Recommendation Engine
> Collaborative filtering: user yang beli A juga beli B → recommend B ke user baru

**11.** Design Multi-tenant SaaS
> Schema per tenant (isolation) atau Row-level security (cost-effective)

**12.** Design API dengan Versioning
> URL versioning (/api/v1, /api/v2) atau Header versioning (Api-Version: 2.0)

**13.** Design Caching Strategy untuk E-commerce
> Cache product catalog (high read, low write), invalidate on update, TTL 5-60 menit

**14.** Design Distributed Lock
> Redis SETNX untuk distributed lock, Redlock untuk multi-node Redis

**15.** Design Blue-Green Deployment
> Dua production environment, switch traffic router, zero downtime deployment
