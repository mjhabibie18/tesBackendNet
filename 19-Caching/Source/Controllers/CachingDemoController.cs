// ============================================================
// Nama File: CachingDemoController.cs — Controller Demo In-Memory & Redis Cache
// Folder: 19-Caching/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Caching):
//    - Tujuan: Menerapkan pola penyimpanan data sementara (cache) guna memotong latensi kueri database.
//    - Kapan Digunakan: Saat data sering dibaca tapi jarang berubah (seperti daftar produk, konfigurasi, detail profil).
//    - Hubungan: Menjadi layer pelindung performa sebelum kueri dialirkan ke Database.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan HTTP API endpoints untuk memicu simulasi Cache-Aside Pattern.
//    - Mengapa Diperlukan: Sebagai contoh konkrit penggunaan IMemoryCache (lokal RAM) dan IDistributedCache (Redis).
//    - Hubungan File: Memanggil Product.cs untuk tipe data entitas dan dikonfigurasi di Program.cs.
//    - Jika Dihapus: Sistem akan kehilangan demonstrasi REST API caching dan tes fungsional caching tidak bisa dilakukan.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using TesBackendNet.Caching.Models;

namespace TesBackendNet.Caching.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller API yang mendemonstrasikan Cache-Aside Pattern (juga dikenal sebagai Lazy Loading).
/// 
/// ALASAN MENGGUNAKAN DUA KATEGORI CACHING:
/// 1. IMemoryCache (In-Memory): Menyimpan data langsung di memori RAM server aplikasi saat ini.
///    - Kelebihan: Sangat cepat (mikrodetik).
///    - Kekurangan: Data hilang jika server restart, dan tidak tersinkronisasi jika kita memiliki multiple servers (Load Balanced).
/// 2. IDistributedCache (Distributed): Menyimpan data secara eksternal (biasanya Redis Server).
///    - Kelebihan: Tersinkronisasi antar-server, data tetap persisten meskipun server web restart.
///    - Kekurangan: Memerlukan koneksi I/O jaringan tambahan (latensi milidetik).
/// 
/// DEPENDENCY:
/// - IMemoryCache: Interface bawaan .NET untuk manipulasi cache memori lokal.
/// - IDistributedCache: Interface bawaan .NET untuk manipulasi distributed cache (Redis/SQL Server/NCache).
/// </summary>
[ApiController]
[Route("api/caching")]
public class CachingDemoController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    // Kunci unik untuk membedakan namespace penyimpanan di memori
    private const string MemoryCacheKey = "products_memory_key";
    private const string DistributedCacheKey = "products_distributed_key";

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan IMemoryCache dan IDistributedCache ke Controller.
    /// </summary>
    public CachingDemoController(IMemoryCache memoryCache, IDistributedCache distributedCache)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    // ================================================================
    // 1. IN-MEMORY CACHE (IMemoryCache)
    // ================================================================

    /// <summary>
    /// FUNGSI METHOD: Mengambil daftar produk dari In-Memory Cache.
    /// NILAI KEMBALIAN: HTTP 200 OK beserta data produk dan keterangan sumber data (Cache Hit / Miss).
    /// 
    /// ALUR EKSEKUSI CACHE-ASIDE (IMemoryCache):
    /// 1. Sistem memeriksa keberadaan data di memori menggunakan `TryGetValue`.
    /// 2. Jika ada (Cache Hit): Data langsung dikembalikan ke klien (sangat cepat).
    /// 3. Jika tidak ada (Cache Miss):
    ///    - Data ditarik secara manual dari dummy database (delay 1 detik).
    ///    - Data disimpan kembali ke dalam cache untuk request berikutnya dengan opsi Expiration.
    ///    - Mengembalikan data ke klien dengan tanda "Database (Cache Miss)".
    /// 
    /// PEMAHAMAN ABSOLUTE VS SLIDING EXPIRATION:
    /// - Absolute Expiration (30 detik): Data pasti kadaluarsa dan dihapus setelah 30 detik sejak dibuat.
    /// - Sliding Expiration (15 detik): Jika data diakses dalam waktu 15 detik, umur cache di-extend 15 detik lagi.
    ///   Namun, sliding expiration tidak akan melampaui batas waktu Absolute Expiration demi menghindari data basi yang tak pernah mati.
    /// </summary>
    [HttpGet("memory")]
    public IActionResult GetFromMemoryCache()
    {
        Console.WriteLine("[Memory Cache] Memeriksa data di cache...");

        // TryGetValue mencoba mengambil data berdasarkan key. out products akan terisi jika key ditemukan.
        if (!_memoryCache.TryGetValue(MemoryCacheKey, out List<Product>? products))
        {
            Console.WriteLine("[Memory Cache] CACHE MISS. Mengambil data dari database dummy...");
            
            // Mengambil dari database tiruan
            products = GetDummyDataFromDatabase();

            // Mengonfigurasi masa berlaku cache
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30))
                .SetSlidingExpiration(TimeSpan.FromSeconds(15));

            // Menyimpan objek langsung di memori RAM server
            _memoryCache.Set(MemoryCacheKey, products, cacheEntryOptions);
            
            return Ok(new { Source = "Database (Cache Miss)", Data = products });
        }

        Console.WriteLine("[Memory Cache] CACHE HIT. Mengembalikan data dari memory...");
        return Ok(new { Source = "In-Memory Cache (Cache Hit)", Data = products });
    }

    /// <summary>
    /// FUNGSI METHOD: Menghapus (invalidate) cache lokal secara paksa.
    /// KAPAN DIGUNAKAN: Ketika ada pembaruan data (Update/Create/Delete) pada produk asli di database.
    /// </summary>
    [HttpDelete("memory/invalidate")]
    public IActionResult InvalidateMemoryCache()
    {
        // Menggunakan method Remove untuk membuang key dari memori
        _memoryCache.Remove(MemoryCacheKey);
        Console.WriteLine("[Memory Cache] Cache dibersihkan.");
        return Ok(new { Message = "In-Memory Cache berhasil dibersihkan (Invalidated)." });
    }

    // ================================================================
    // 2. DISTRIBUTED CACHE (IDistributedCache - Redis/Memory)
    // ================================================================

    /// <summary>
    /// FUNGSI METHOD: Mengambil daftar produk dari Distributed Cache (Redis/Memory).
    /// NILAI KEMBALIAN: Task<IActionResult> karena operasi distributed cache memerlukan I/O asinkron.
    /// 
    /// MENGAPA MENGGUNAKAN SERIALISASI JSON?
    /// Berbeda dengan IMemoryCache yang dapat menyimpan Object C# secara langsung di RAM lokal,
    /// IDistributedCache menyimpan data dalam bentuk byte array atau string JSON. Oleh karena itu, kita harus:
    /// - Melakukan Serialize (Object -> String JSON) saat menyimpan.
    /// - Melakukan Deserialize (String JSON -> Object) saat membaca.
    /// </summary>
    [HttpGet("distributed")]
    public async Task<IActionResult> GetFromDistributedCache()
    {
        Console.WriteLine("[Distributed Cache] Memeriksa data di Redis/Memory...");

        // Membaca string JSON dari penyimpanan distributed secara asinkron
        var cachedJson = await _distributedCache.GetStringAsync(DistributedCacheKey);

        if (string.IsNullOrEmpty(cachedJson))
        {
            Console.WriteLine("[Distributed Cache] CACHE MISS. Mengambil dari database dummy...");
            
            var products = GetDummyDataFromDatabase();

            // Mengonversi List<Product> menjadi format string JSON terkompresi
            var jsonString = JsonSerializer.Serialize(products);

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
                SlidingExpiration = TimeSpan.FromSeconds(30)
            };

            // Menulis ke distributed storage secara asinkron
            await _distributedCache.SetStringAsync(DistributedCacheKey, jsonString, options);
            
            return Ok(new { Source = "Database (Cache Miss)", Data = products });
        }

        Console.WriteLine("[Distributed Cache] CACHE HIT. Mengembalikan data JSON dari Distributed Storage...");
        
        // Membaca string JSON dan mengembalikannya ke bentuk objek list C#
        var cachedProducts = JsonSerializer.Deserialize<List<Product>>(cachedJson);
        
        return Ok(new { Source = "Distributed Cache (Cache Hit)", Data = cachedProducts });
    }

    /// <summary>
    /// FUNGSI METHOD: Menghapus (invalidate) distributed cache secara asinkron.
    /// </summary>
    [HttpDelete("distributed/invalidate")]
    public async Task<IActionResult> InvalidateDistributedCache()
    {
        // Menghapus data dari distributed cache
        await _distributedCache.RemoveAsync(DistributedCacheKey);
        Console.WriteLine("[Distributed Cache] Cache dibersihkan.");
        return Ok(new { Message = "Distributed Cache berhasil dibersihkan (Invalidated)." });
    }

    // ── Helper Dummy Database Query ──────────────────────────────────
    /// <summary>
    /// FUNGSI METHOD: Simulasi operasi kueri database yang lambat.
    /// </summary>
    private static List<Product> GetDummyDataFromDatabase()
    {
        // Thread.Sleep(1000) memblokir thread selama 1 detik untuk menyimulasikan overhead jaringan database.
        Thread.Sleep(1000); 
        return new List<Product>
        {
            new Product { Id = 1, Name = "Televisi LED 43 Inch", Price = 4200000 },
            new Product { Id = 2, Name = "Air Conditioner 1 PK", Price = 3800000 },
            new Product { Id = 3, Name = "Kulkas 2 Pintu", Price = 5100000 }
        };
    }
}
