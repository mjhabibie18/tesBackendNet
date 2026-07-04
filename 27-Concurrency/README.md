# ⚡ 27 — Concurrency & Async Programming

## Concurrency vs Parallelism

```
Concurrency:  Menangani banyak task sekaligus (bisa sequential)
              Contoh: waiter yang handle banyak meja bergantian

Parallelism:  Eksekusi task secara benar-benar bersamaan (multi-core)
              Contoh: banyak waiter untuk banyak meja
```

---

## Async/Await

```csharp
// ❌ SINKRON: thread blocked saat menunggu database
public ProductDto GetProduct(int id)
{
    var product = _repo.GetById(id); // Thread blocked! ❌
    return MapToDto(product);
}

// ✅ ASINKRON: thread bisa handle request lain saat menunggu
public async Task<ProductDto> GetProductAsync(int id)
{
    var product = await _repo.GetByIdAsync(id); // Thread bebas! ✅
    return MapToDto(product);
}
```

---

## Task Parallel Library (TPL)

```csharp
// Jalankan beberapa task paralel
var productsTask  = _productRepo.GetAllAsync();
var categoriesTask = _categoryRepo.GetAllAsync();

// Tunggu keduanya selesai bersamaan
await Task.WhenAll(productsTask, categoriesTask);

var products   = productsTask.Result;
var categories = categoriesTask.Result;

// Task.WhenAny: tunggu salah satu selesai
var firstTask = await Task.WhenAny(task1, task2, task3);
```

---

## CancellationToken

```csharp
// Batalkan operasi jika client disconnect
[HttpGet]
public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
{
    var result = await _service.GetAllAsync(cancellationToken);
    return Ok(result);
}

// Di Repository
public async Task<List<Product>> GetAllAsync(CancellationToken ct = default)
{
    return await _context.Products
        .ToListAsync(ct); // Dibatalkan jika ct di-cancel
}
```

---

## Race Condition & Lock

```csharp
// ❌ Race Condition
private int _counter = 0;
public void Increment()
{
    _counter++; // Tidak thread-safe!
}

// ✅ Interlocked untuk operasi atomic
public void Increment()
{
    Interlocked.Increment(ref _counter);
}

// ✅ Lock untuk critical section yang kompleks
private readonly object _lock = new();
public void UpdateBalance(decimal amount)
{
    lock (_lock)
    {
        _balance += amount; // Thread-safe
    }
}

// ✅ SemaphoreSlim untuk async lock
private readonly SemaphoreSlim _semaphore = new(1, 1);
public async Task UpdateAsync()
{
    await _semaphore.WaitAsync();
    try { /* critical section */ }
    finally { _semaphore.Release(); }
}
```

---

## 🎤 Tips Interview

**Q: "Apa itu deadlock?"**
```
Deadlock: dua thread saling menunggu resource yang dipegang thread lain.
Thread A menunggu Lock B, Thread B menunggu Lock A → stuck forever!

Solusi:
1. Urutan lock yang konsisten
2. Timeout pada lock
3. Gunakan async/await (tidak memblok thread)
```

**Q: "Apa bedanya Task dan Thread?"**
```
Thread: unit eksekusi OS (berat, ~1MB stack)
Task: abstraksi yang berjalan di ThreadPool (ringan)
→ Untuk I/O async: Task (tidak butuh dedicated thread)
→ Untuk CPU-bound: Thread atau Task.Run()
```
