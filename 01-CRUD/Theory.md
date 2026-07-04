# 📖 Theory — CRUD (Create, Read, Update, Delete)

## 1. Konsep Dasar CRUD

CRUD adalah empat operasi fundamental dalam pengelolaan data:

```
┌─────────────┬────────────────┬──────────────┬────────────────────┐
│  Operasi    │   HTTP Method  │  SQL Command │  EF Core           │
├─────────────┼────────────────┼──────────────┼────────────────────┤
│  Create     │   POST         │  INSERT      │  Add() + SaveAsync │
│  Read All   │   GET          │  SELECT *    │  ToListAsync()     │
│  Read One   │   GET /:id     │  SELECT WHERE│  FindAsync()       │
│  Update     │   PUT / PATCH  │  UPDATE      │  Update()          │
│  Delete     │   DELETE /:id  │  DELETE      │  Remove()          │
└─────────────┴────────────────┴──────────────┴────────────────────┘
```

---

## 2. Arsitektur Clean CRUD

### Alur Request Lengkap

```
HTTP Request
     │
     ▼
┌──────────────────────────────────┐
│          Program.cs              │
│  (Middleware Pipeline Setup)     │
└──────────────────┬───────────────┘
                   │
                   ▼
┌──────────────────────────────────┐
│         Middleware               │
│  (Auth, Logging, CORS, dll)      │
└──────────────────┬───────────────┘
                   │
                   ▼
┌──────────────────────────────────┐
│         Controller               │
│  - Route: /api/products          │
│  - HTTP Method: GET/POST/etc     │
│  - Terima HttpRequest            │
│  - Panggil Service               │
│  - Return HTTP Response          │
└──────────────────┬───────────────┘
                   │
                   ▼
┌──────────────────────────────────┐
│           Service                │
│  - Business Logic                │
│  - Validasi bisnis               │
│  - Transform data (DTO mapping)  │
│  - Panggil Repository            │
└──────────────────┬───────────────┘
                   │
                   ▼
┌──────────────────────────────────┐
│          Repository              │
│  - Akses Database                │
│  - Pakai DbContext (EF Core)     │
│  - Query SQL via LINQ            │
└──────────────────┬───────────────┘
                   │
                   ▼
┌──────────────────────────────────┐
│         SQL Server               │
│  (Database)                      │
└──────────────────────────────────┘
```

---

## 3. Separation of Concerns (SoC)

### ❌ Tanpa SoC (Anti-pattern)

```csharp
// JANGAN LAKUKAN INI — semua di Controller
[HttpPost]
public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
{
    // Validasi langsung di Controller ❌
    if (string.IsNullOrEmpty(dto.Name))
        return BadRequest("Name required");

    // Business logic di Controller ❌
    if (dto.Price < 0)
        return BadRequest("Price cannot be negative");

    // Database access langsung di Controller ❌
    var product = new Product
    {
        Name = dto.Name,
        Price = dto.Price
    };
    _context.Products.Add(product);
    await _context.SaveChangesAsync();

    return Ok(product);
}
```

**Masalah:**
- Controller tahu terlalu banyak
- Susah di-unit test
- Susah diganti database-nya
- Duplikasi kode jika ada endpoint lain

---

### ✅ Dengan SoC (Clean Architecture)

```csharp
// Controller — hanya handle HTTP
[HttpPost]
public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
{
    var result = await _productService.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
}

// Service — hanya business logic
public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
{
    // Validasi bisnis
    var exists = await _repository.ExistsByNameAsync(dto.Name);
    if (exists) throw new ConflictException($"Product '{dto.Name}' sudah ada");

    // Map DTO → Entity
    var product = _mapper.Map<Product>(dto);

    // Simpan via Repository
    await _repository.AddAsync(product);
    return _mapper.Map<ProductResponseDto>(product);
}

// Repository — hanya akses database
public async Task AddAsync(Product product)
{
    await _context.Products.AddAsync(product);
    await _context.SaveChangesAsync();
}
```

---

## 4. Model vs DTO

### Model / Entity
Merepresentasikan tabel di database:

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; }           // Soft delete
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

### DTO (Data Transfer Object)
Object untuk transfer data, berbeda dari Entity:

```csharp
// Request DTO — untuk CREATE
public class ProductCreateDto
{
    public string Name { get; set; }          // Wajib
    public string? Description { get; set; }  // Opsional
    public decimal Price { get; set; }
    public int Stock { get; set; }
    // TIDAK ADA: Id, IsDeleted, CreatedAt, UpdatedAt
    // karena ini di-set oleh server, bukan client
}

// Request DTO — untuk UPDATE
public class ProductUpdateDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    // TIDAK ADA: Id (dari URL), IsDeleted, timestamp
}

// Response DTO — untuk READ
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
    // TIDAK ADA: IsDeleted (internal), UpdatedBy (sensitive)
}
```

### Mengapa Perlu DTO?

```
1. Keamanan: Tidak expose field sensitif (password, internal flag)
2. Fleksibilitas: Response bisa berbeda dari struktur database
3. Versioning: Bisa buat v1, v2 DTO tanpa ubah Model
4. Validasi: Validasi bisa dilakukan di level DTO
5. Dokumentasi: Swagger lebih bersih dan informatif
```

---

## 5. Soft Delete

### Konsep

```
Hard Delete:
Database: DELETE FROM Products WHERE Id = 1
→ Data HILANG PERMANEN ❌

Soft Delete:
Database: UPDATE Products SET IsDeleted = 1 WHERE Id = 1
→ Data MASIH ADA, hanya ditandai ✅
```

### Implementasi

```csharp
// Model — tambahkan field soft delete
public class Product
{
    // ... fields lain ...
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

// Repository — filter data yang tidak dihapus
public async Task<List<Product>> GetAllAsync()
{
    return await _context.Products
        .Where(p => !p.IsDeleted)  // ← PENTING: filter soft delete
        .ToListAsync();
}

// Repository — soft delete
public async Task SoftDeleteAsync(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product != null)
    {
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
```

### Global Query Filter (Best Practice)

```csharp
// Di DbContext — filter soft delete berlaku OTOMATIS
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Semua query Product otomatis filter IsDeleted = false
    modelBuilder.Entity<Product>()
        .HasQueryFilter(p => !p.IsDeleted);
}
```

---

## 6. Pagination

### Konsep

```
Tanpa Pagination:
GET /api/products → return SEMUA data (bisa jutaan record!) ❌

Dengan Pagination:
GET /api/products?page=1&pageSize=10 → return 10 data pertama ✅
GET /api/products?page=2&pageSize=10 → return 10 data berikutnya ✅
```

### Parameter Pagination

```
page     → Nomor halaman (default: 1)
pageSize → Jumlah data per halaman (default: 10, max: 100)
```

### SQL Translation

```sql
-- Page 1, PageSize 10
SELECT * FROM Products
ORDER BY Id
OFFSET 0 ROWS           -- (page-1) * pageSize
FETCH NEXT 10 ROWS ONLY -- pageSize

-- Page 2, PageSize 10
SELECT * FROM Products
ORDER BY Id
OFFSET 10 ROWS
FETCH NEXT 10 ROWS ONLY
```

### LINQ Translation

```csharp
var query = _context.Products.AsQueryable();

// Hitung total data
var totalCount = await query.CountAsync();

// Apply pagination
var products = await query
    .Skip((page - 1) * pageSize)   // OFFSET
    .Take(pageSize)                  // FETCH NEXT
    .ToListAsync();
```

### Response Pagination

```json
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---

## 7. Search, Filter, Sort

### Search

```csharp
// Cari berdasarkan keyword di Name atau Description
if (!string.IsNullOrEmpty(search))
{
    query = query.Where(p =>
        p.Name.Contains(search) ||
        p.Description.Contains(search));
}
```

### Filter

```csharp
// Filter berdasarkan range harga
if (minPrice.HasValue)
    query = query.Where(p => p.Price >= minPrice.Value);

if (maxPrice.HasValue)
    query = query.Where(p => p.Price <= maxPrice.Value);

// Filter berdasarkan kategori
if (categoryId.HasValue)
    query = query.Where(p => p.CategoryId == categoryId.Value);
```

### Sort

```csharp
// Dynamic sorting
query = sortBy?.ToLower() switch
{
    "name"  => sortDesc ? query.OrderByDescending(p => p.Name)
                        : query.OrderBy(p => p.Name),
    "price" => sortDesc ? query.OrderByDescending(p => p.Price)
                        : query.OrderBy(p => p.Price),
    _       => query.OrderBy(p => p.Id)  // default sort
};
```

---

## 8. HTTP Status Codes untuk CRUD

```
┌──────────┬──────────────────┬────────────────────────────────┐
│  Code    │  Nama            │  Kapan Digunakan               │
├──────────┼──────────────────┼────────────────────────────────┤
│  200 OK  │  OK              │  GET berhasil                  │
│  201     │  Created         │  POST berhasil membuat data    │
│  204     │  No Content      │  DELETE berhasil               │
│  400     │  Bad Request     │  Input tidak valid             │
│  404     │  Not Found       │  Data tidak ditemukan          │
│  409     │  Conflict        │  Data sudah ada (duplicate)    │
│  422     │  Unprocessable   │  Validasi gagal                │
│  500     │  Server Error    │  Error tidak terduga           │
└──────────┴──────────────────┴────────────────────────────────┘
```

---

## 9. Repository Pattern vs Direct DbContext

### Tanpa Repository Pattern

```csharp
// Service langsung pakai DbContext
public class ProductService
{
    private readonly AppDbContext _context;  // ← Tergantung langsung ke EF Core

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }
}

// Masalah:
// 1. Susah di-unit test (DbContext sulit di-mock)
// 2. Business logic dan data access tercampur
// 3. Ganti database = ubah semua Service
```

### Dengan Repository Pattern

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

// Implementation — EF Core
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public async Task<List<Product>> GetAllAsync()
        => await _context.Products.ToListAsync();
}

// Service — tidak tahu EF Core
public class ProductService
{
    private readonly IProductRepository _repository;  // ← Pakai interface

    // Saat unit test: bisa inject MockRepository
    // Saat production: inject ProductRepository
}
```

---

## 10. Diagram Alur CRUD Lengkap

```
┌─────────────────────────────────────────────────┐
│                   CLIENT                        │
│         (Browser, Postman, Mobile App)          │
└─────────────────┬───────────────────────────────┘
                  │
         HTTP Request dengan:
         - Method (GET/POST/PUT/DELETE)
         - URL (/api/products)
         - Body (JSON untuk POST/PUT)
         - Headers (Content-Type, Authorization)
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│              ASP.NET Core Pipeline              │
│  1. Routing → cocokkan URL ke Controller        │
│  2. Model Binding → bind JSON ke C# object      │
│  3. Model Validation → validasi DataAnnotations │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│              ProductController                  │
│  [HttpGet] GetAll()                             │
│  [HttpGet("{id}")] GetById()                    │
│  [HttpPost] Create()                            │
│  [HttpPut("{id}")] Update()                     │
│  [HttpDelete("{id}")] Delete()                  │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│               IProductService                   │
│  GetAllAsync(query params)                      │
│  GetByIdAsync(id)                               │
│  CreateAsync(dto)                               │
│  UpdateAsync(id, dto)                           │
│  DeleteAsync(id)                                │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│             IProductRepository                  │
│  GetAllAsync(filter, sort, page)                │
│  GetByIdAsync(id)                               │
│  AddAsync(entity)                               │
│  UpdateAsync(entity)                            │
│  SoftDeleteAsync(id)                            │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│            AppDbContext (EF Core)               │
│  DbSet<Product> Products                        │
└─────────────────┬───────────────────────────────┘
                  │
                  ▼
┌─────────────────────────────────────────────────┐
│              SQL Server Database                │
│  Table: Products                                │
└─────────────────────────────────────────────────┘
```
