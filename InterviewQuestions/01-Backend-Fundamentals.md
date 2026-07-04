# 🎤 01 — Backend Fundamentals Interview Questions

## C#, OOP, ASP.NET Core, REST API

---

## Soal 1: Apa itu Dependency Injection?

**Tingkat**: Easy | **Topik**: DI, OOP

### Deskripsi
Jelaskan apa itu Dependency Injection dan mengapa digunakan.

### Solusi

**Dependency Injection (DI)** adalah teknik di mana dependensi sebuah class diberikan dari luar (inject), bukan dibuat sendiri di dalam class.

### Kode Lengkap

```csharp
// ❌ Tanpa DI: tight coupling
public class OrderService
{
    private ProductRepository _repo = new ProductRepository(); // Tight coupling!
    // Susah ganti implementasi, susah test
}

// ✅ Dengan DI: loose coupling
public class OrderService
{
    private readonly IProductRepository _repo;

    public OrderService(IProductRepository repo) // Inject dari luar
    {
        _repo = repo;
    }
}

// Register di Program.cs
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

### Mengapa DI?

```
1. Loose Coupling: class tidak terikat pada implementasi spesifik
2. Testability: bisa mock dependency saat unit test
3. Maintainability: ganti implementasi tanpa ubah code yang pakai
4. Separation of Concerns: class fokus pada satu tanggung jawab
```

---

## Soal 2: Jelaskan perbedaan interface dan abstract class!

**Tingkat**: Easy | **Topik**: OOP, C#

### Solusi

| | Interface | Abstract Class |
|--|-----------|---------------|
| **Dapat di-inherit** | Multiple | Single |
| **Field** | Tidak boleh | Boleh |
| **Constructor** | Tidak boleh | Boleh |
| **Access Modifier** | Public (default) | Bebas |
| **Implementasi** | Tidak ada (default bisa sejak C# 8) | Boleh sebagian |
| **Kapan** | Kontrak murni | Base class dengan shared logic |

### Kode Lengkap

```csharp
// Interface — kontrak murni
public interface IPaymentProcessor
{
    Task<bool> ProcessAsync(decimal amount);
    // Tidak ada implementasi (sebelum C# 8)
}

// Abstract class — base class dengan shared logic
public abstract class PaymentBase
{
    protected readonly ILogger _logger;

    protected PaymentBase(ILogger logger)
        => _logger = logger;

    // Method abstract: WAJIB diimplementasi turunan
    public abstract Task<bool> ProcessAsync(decimal amount);

    // Method konkrit: bisa dipakai langsung oleh turunan
    protected void LogTransaction(decimal amount)
        => _logger.LogInformation("Processing {Amount}", amount);
}

// Implementasi interface
public class StripeProcessor : IPaymentProcessor
{
    public async Task<bool> ProcessAsync(decimal amount) { ... }
}

// Implementasi multiple interface
public class MidtransProcessor : IPaymentProcessor, IRefundable, IRecurring
{
    public async Task<bool> ProcessAsync(decimal amount) { ... }
    public async Task<bool> RefundAsync(string transactionId) { ... }
    public async Task ScheduleAsync(decimal amount, DateTime date) { ... }
}
```

---

## Soal 3: Jelaskan SOLID Principles!

**Tingkat**: Medium | **Topik**: OOP, Clean Code

### Solusi

```csharp
// S — Single Responsibility Principle
// ❌ Satu class banyak tanggung jawab
public class User
{
    public void Save() { /* database */ }
    public void SendEmail() { /* email */ }
    public void Validate() { /* validation */ }
}

// ✅ Satu class, satu tanggung jawab
public class UserRepository { public void Save() { } }
public class EmailService { public void SendEmail() { } }
public class UserValidator { public void Validate() { } }

// O — Open/Closed Principle
// Open for extension, closed for modification
public abstract class Shape { public abstract double Area(); }
public class Circle : Shape { public override double Area() => Math.PI * r * r; }
public class Square : Shape { public override double Area() => side * side; }
// Tambah shape baru? Buat class baru, tidak ubah yang lama

// L — Liskov Substitution Principle
// Subclass bisa replace parent tanpa ubah behavior
Animal animal = new Dog(); // Dog extends Animal
animal.Speak(); // Bekerja dengan benar

// I — Interface Segregation Principle
// Interface kecil dan spesifik
public interface IPrintable { void Print(); }
public interface IScannable { void Scan(); }
// Bukan: interface IGodPrinter { void Print(); void Scan(); void Fax(); }

// D — Dependency Inversion Principle
// Depend on abstraction, not implementation
public class ProductService
{
    private readonly IProductRepository _repo; // Interface, bukan class!
}
```

---

## Soal 4: Apa bedanya Stack dan Heap?

**Tingkat**: Medium | **Topik**: C#, Memory Management

### Solusi

```
STACK:
- Menyimpan value types (int, bool, double, struct)
- Menyimpan reference ke heap (pointer)
- LIFO (Last In, First Out)
- Alokasi/dealokasi otomatis (scope-based)
- Sangat cepat, ukuran terbatas (~1MB default)

HEAP:
- Menyimpan reference types (class, array, string)
- Dikelola oleh Garbage Collector
- Lebih lambat dari stack
- Ukuran besar (RAM tersedia)

Contoh:
int x = 5;               // x ada di STACK
var p = new Product();   // p (reference) di STACK, object di HEAP
```

---

## Soal 5: Jelaskan Garbage Collection di .NET!

**Tingkat**: Medium | **Topik**: .NET Runtime

### Solusi

```
GC (Garbage Collector) otomatis membebaskan memori yang tidak digunakan.

Generasi:
Gen 0: Object baru (short-lived) — dikumpulkan paling sering
Gen 1: Object yang selamat dari Gen 0
Gen 2: Object long-lived (static, cache) — dikumpulkan paling jarang

Cara bantu GC:
- Dispose object yang implement IDisposable
- Gunakan using statement
- Hindari memory leak (event handler yang tidak di-unsubscribe)
```

```csharp
// Gunakan using untuk auto-dispose
using var connection = new SqlConnection(connStr);
// Setelah using block, connection.Dispose() dipanggil otomatis

// Atau manual dispose
var connection = new SqlConnection(connStr);
try { /* ... */ }
finally { connection.Dispose(); }
```

---

## Soal 6: Buat REST API CRUD untuk Product (Coding Test)

**Tingkat**: Medium | **Topik**: CRUD, REST, EF Core

### Deskripsi

Buat REST API untuk manajemen Product dengan:
- GET /api/products (list dengan pagination)
- GET /api/products/{id}
- POST /api/products
- PUT /api/products/{id}
- DELETE /api/products/{id} (soft delete)

### Solusi

Lihat implementasi lengkap di: [../01-CRUD/Source/](../01-CRUD/Source/)

### Kode Lengkap (Ringkasan)

```csharp
// Model
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// Controller
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    public ProductController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
        => Ok(await _service.GetAllAsync(query));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        => Ok(await _service.UpdateAsync(id, dto));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(new { message = "Deleted" });
    }
}
```

---

## Soal 7: Apa itu async/await?

**Tingkat**: Medium | **Topik**: C#, Concurrency

### Solusi

```csharp
// SYNCHRONOUS: thread blocked saat menunggu I/O
public ProductDto GetProduct(int id)
{
    var product = dbContext.Products.Find(id); // Thread blocked!
    return MapToDto(product);
}

// ASYNCHRONOUS: thread tidak blocked, bisa handle request lain
public async Task<ProductDto> GetProductAsync(int id)
{
    var product = await dbContext.Products.FindAsync(id); // Thread bebas!
    return MapToDto(product);
}
```

**Mengapa async?**
```
Server web punya thread pool terbatas (misal 100 thread).
Jika setiap request blocked 500ms untuk database query,
dengan 100 thread = hanya bisa handle 200 req/detik.

Dengan async: thread dikembalikan ke pool saat await,
bisa handle ribuan request concurrent.
```

---

## Soal 8: Apa itu Middleware di ASP.NET Core?

**Tingkat**: Medium | **Topik**: ASP.NET Core

### Solusi

```
Middleware adalah komponen yang memproses HTTP request/response.
Dieksekusi secara berurutan (pipeline).

Request:  MW1 → MW2 → MW3 → Endpoint
Response: Endpoint → MW3 → MW2 → MW1

Contoh middleware built-in:
- UseHttpsRedirection
- UseAuthentication
- UseAuthorization
- UseStaticFiles

Custom middleware:
```

```csharp
public class TimingMiddleware
{
    private readonly RequestDelegate _next;

    public TimingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        await _next(context); // → next middleware

        sw.Stop();
        Console.WriteLine($"Request took {sw.ElapsedMilliseconds}ms");
    }
}

app.UseMiddleware<TimingMiddleware>();
```

---

## Soal 9: Jelaskan Repository Pattern!

**Tingkat**: Medium | **Topik**: Design Pattern

### Solusi

Repository Pattern memisahkan data access logic dari business logic.

```csharp
// Interface — kontrak
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
}

// Implementation — detail EF Core
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<List<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();

    // ... implementasi lainnya
}
```

**Mengapa Repository Pattern?**
```
1. Separation of Concerns
2. Testability (bisa mock)
3. Flexibility (ganti ORM/database)
4. Reusability (query yang sama tidak duplikat)
```

---

## Soal 10: Apa itu IActionResult vs ActionResult?

**Tingkat**: Easy | **Topik**: ASP.NET Core

### Solusi

```csharp
// IActionResult: interface, lebih fleksibel
// Bisa return berbagai tipe response
public async Task<IActionResult> GetProduct(int id)
{
    var product = await _service.GetByIdAsync(id);
    if (product == null)
        return NotFound(); // 404
    return Ok(product);   // 200
}

// ActionResult<T>: generic, lebih type-safe
// Swagger bisa generate response schema yang akurat
public async Task<ActionResult<ProductDto>> GetProduct(int id)
{
    var product = await _service.GetByIdAsync(id);
    if (product == null)
        return NotFound();  // implicit cast ke ActionResult
    return product;          // implicit cast ke OkObjectResult
}

// Recommendation: gunakan IActionResult untuk kemudahan
// Gunakan ActionResult<T> jika butuh type-safe + Swagger schema
```

---

## Soal 11–30: (Ringkasan)

**11.** Apa bedanya `FirstOrDefault` dan `Find` di EF Core?
> Find: cek identity map cache dulu, FindAsync lebih efisien untuk PK lookup
> FirstOrDefault: selalu query database, support kondisi complex

**12.** Bagaimana cara handle concurrent requests?
> async/await, thread pool, CancellationToken

**13.** Apa itu Generic type di C#?
> `List<T>`, `Task<T>`, `IRepository<T>` — type yang flexible

**14.** Apa bedanya `IEnumerable`, `ICollection`, `IList`?
> IEnumerable: read-only, forward-only (query belum dieksekusi)
> ICollection: + Count, Add, Remove
> IList: + access by index

**15.** Jelaskan Nullable reference types di C# 8+!
> `string?` = bisa null, `string` = tidak boleh null, compiler warning

**16.** Apa itu Record type di C#?
> Immutable value-like class, good for DTO: `record ProductDto(int Id, string Name);`

**17.** Bagaimana cara membuat Custom Attribute?
> Turunkan dari `Attribute` class, bisa dipakai untuk metadata

**18.** Apa itu Extension Method?
> Method yang "ditambahkan" ke type yang tidak bisa dimodifikasi:
> `public static string ToSnakeCase(this string s) { ... }`

**19.** Apa itu LINQ?
> Language Integrated Query: cara query data dalam C# dengan syntax SQL-like

**20.** Apa itu HttpContext?
> Object yang berisi semua informasi request/response HTTP saat ini

**21.** Bagaimana cara protect endpoint dari CSRF?
> JWT Bearer otomatis terlindungi (tidak ada cookie). Untuk form: gunakan AntiForgery token

**22.** Apa itu ProblemDetails?
> Standar RFC 7807 untuk format error response: type, title, status, detail

**23.** Bagaimana cara membuat Background Service?
> Turunkan dari `BackgroundService`, override `ExecuteAsync`, register `AddHostedService<T>()`

**24.** Apa itu SignalR?
> Library untuk real-time communication (WebSocket) antara server dan client

**25.** Bagaimana cara implement Health Check?
> `builder.Services.AddHealthChecks()`, `app.MapHealthChecks("/health")`

**26.** Apa itu gRPC vs REST?
> gRPC: binary, strongly-typed (protobuf), cocok untuk microservices internal
> REST: JSON, human-readable, cocok untuk public API

**27.** Bagaimana cara implement Rate Limiting di ASP.NET Core 7+?
> Built-in: `builder.Services.AddRateLimiter()`, `app.UseRateLimiter()`

**28.** Apa itu YARP (Yet Another Reverse Proxy)?
> Library Microsoft untuk API Gateway dalam .NET ecosystem

**29.** Apa itu Polly?
> Library untuk resilience: retry, circuit breaker, timeout, fallback

**30.** Bagaimana cara implement Idempotency di API?
> Idempotency Key di request header, cek di database sebelum proses
