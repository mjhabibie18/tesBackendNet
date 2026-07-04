# ⚡ 19 — Caching

## Apa itu Caching?

Caching adalah teknik menyimpan data di memori sementara agar request berikutnya lebih cepat.

```
Tanpa cache:
Request → Server → Database → Response (100-500ms)

Dengan cache:
Request → Cache HIT → Response (1-5ms) ← 100x lebih cepat!
```

---

## Jenis Cache

| Jenis | Storage | Scope | Contoh |
|-------|---------|-------|--------|
| **In-Memory** | RAM server | Satu server | IMemoryCache |
| **Distributed** | Redis/Memcached | Multi-server | IDistributedCache + Redis |
| **Response Cache** | Client/CDN | Browser | [ResponseCache] |

---

## 1. In-Memory Cache

```csharp
// Program.cs
builder.Services.AddMemoryCache();

// Service
public class ProductService
{
    private readonly IMemoryCache _cache;
    private const string CacheKey = "products_all";

    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        // Coba ambil dari cache dulu
        if (_cache.TryGetValue(CacheKey, out List<ProductResponseDto>? cachedProducts))
            return cachedProducts!;

        // Cache miss: ambil dari database
        var products = await _repo.GetAllAsync();
        var dtos = products.Select(MapToDto).ToList();

        // Simpan ke cache dengan expiry 5 menit
        _cache.Set(CacheKey, dtos, TimeSpan.FromMinutes(5));

        return dtos;
    }

    // Invalidate cache saat ada perubahan
    public async Task CreateAsync(ProductCreateDto dto)
    {
        await _repo.AddAsync(/* ... */);
        _cache.Remove(CacheKey); // Hapus cache agar data terbaru
    }
}
```

---

## 2. Redis Distributed Cache

```bash
dotnet add package StackExchange.Redis
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

```csharp
// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName  = "TesBackendNet_";
});

// Service
public class ProductService
{
    private readonly IDistributedCache _cache;

    public async Task<List<ProductResponseDto>?> GetFromCacheAsync()
    {
        var cached = await _cache.GetStringAsync("products");
        if (cached == null) return null;

        return JsonSerializer.Deserialize<List<ProductResponseDto>>(cached);
    }

    public async Task SetCacheAsync(List<ProductResponseDto> data)
    {
        var json = JsonSerializer.Serialize(data);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };
        await _cache.SetStringAsync("products", json, options);
    }
}
```

---

## Cache-Aside Pattern

```
1. App cek cache
2a. Cache HIT → return data dari cache
2b. Cache MISS → query database → simpan ke cache → return data
```

---

## ✅ Best Practice

1. **Cache yang jarang berubah**: produk, kategori, config
2. **Jangan cache data sensitif**: password, token
3. **Set expiry yang tepat**: terlalu lama = stale, terlalu pendek = tidak efektif
4. **Invalidate cache** saat data berubah
5. **Cache key yang jelas**: `products:page:1:pageSize:10`

---

## 🎤 Tips Interview

**Q: "Apa itu Cache Stampede (Thundering Herd)?"**
```
Saat cache expire, banyak request bersamaan hit database.
Solusi: 
1. Probabilistic early expiration
2. Distributed lock saat refresh cache
3. Background refresh sebelum expire
```
