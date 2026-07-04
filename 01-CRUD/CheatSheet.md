# ⚡ CheatSheet — CRUD

> Copy-paste langsung saat coding test!

---

## 🚀 Setup Project (Terminal)

```bash
# Buat project baru
dotnet new webapi -n NamaProject --no-https false

# Masuk ke folder
cd NamaProject

# Install packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Swashbuckle.AspNetCore

# Buat migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update

# Jalankan
dotnet run
```

---

## 📦 Model (Entity)

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
```

---

## 📤 DTO

```csharp
// Create Request
public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// Update Request
public class ProductUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// Response
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## 🗄️ DbContext

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Soft Delete Global Filter
        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.IsDeleted);

        // Seed data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Price = 15000000, Stock = 10, CreatedAt = DateTime.UtcNow },
            new Product { Id = 2, Name = "Mouse", Price = 150000, Stock = 50, CreatedAt = DateTime.UtcNow }
        );
    }
}
```

---

## 🔌 Interface Repository

```csharp
public interface IProductRepository
{
    Task<(List<Product> Data, int TotalCount)> GetAllAsync(
        string? search, string? sortBy, bool sortDesc,
        int page, int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task SoftDeleteAsync(int id);
}
```

---

## 🏗️ Repository Implementation

```csharp
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context) => _context = context;

    public async Task<(List<Product> Data, int TotalCount)> GetAllAsync(
        string? search, string? sortBy, bool sortDesc, int page, int pageSize)
    {
        var query = _context.Products.AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search) ||
                                     (p.Description != null && p.Description.Contains(search)));

        // Sort
        query = sortBy?.ToLower() switch
        {
            "name"  => sortDesc ? query.OrderByDescending(p => p.Name)  : query.OrderBy(p => p.Name),
            "price" => sortDesc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            _       => query.OrderByDescending(p => p.CreatedAt)
        };

        var total = await query.CountAsync();
        var data  = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (data, total);
    }

    public async Task<Product?> GetByIdAsync(int id)
        => await _context.Products.FindAsync(id);

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        => await _context.Products.AnyAsync(p =>
            p.Name == name && (!excludeId.HasValue || p.Id != excludeId.Value));

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return;

        product.IsDeleted  = true;
        product.UpdatedAt  = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }
}
```

---

## ⚙️ Service Interface

```csharp
public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryDto query);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
    Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto dto);
    Task DeleteAsync(int id);
}
```

---

## ⚙️ Service Implementation

```csharp
public class ProductService : IProductService
{
    private readonly IProductRepository _repo;
    public ProductService(IProductRepository repo) => _repo = repo;

    public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryDto query)
    {
        var (data, total) = await _repo.GetAllAsync(
            query.Search, query.SortBy, query.SortDesc,
            query.Page, query.PageSize);

        return new PagedResult<ProductResponseDto>
        {
            Data        = data.Select(MapToDto).ToList(),
            TotalCount  = total,
            Page        = query.Page,
            PageSize    = query.PageSize
        };
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        if (await _repo.ExistsByNameAsync(dto.Name))
            throw new Exception($"Product '{dto.Name}' sudah ada");

        var product = new Product
        {
            Name        = dto.Name,
            Description = dto.Description,
            Price       = dto.Price,
            Stock       = dto.Stock,
            CreatedAt   = DateTime.UtcNow
        };
        await _repo.AddAsync(product);
        return MapToDto(product);
    }

    public async Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto dto)
    {
        var product = await _repo.GetByIdAsync(id)
            ?? throw new Exception($"Product dengan ID {id} tidak ditemukan");

        if (await _repo.ExistsByNameAsync(dto.Name, id))
            throw new Exception($"Product '{dto.Name}' sudah ada");

        product.Name        = dto.Name;
        product.Description = dto.Description;
        product.Price       = dto.Price;
        product.Stock       = dto.Stock;
        product.UpdatedAt   = DateTime.UtcNow;

        await _repo.UpdateAsync(product);
        return MapToDto(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id)
            ?? throw new Exception($"Product dengan ID {id} tidak ditemukan");
        await _repo.SoftDeleteAsync(id);
    }

    // Helper: Map Entity → DTO
    private static ProductResponseDto MapToDto(Product p) => new()
    {
        Id          = p.Id,
        Name        = p.Name,
        Description = p.Description,
        Price       = p.Price,
        Stock       = p.Stock,
        CreatedAt   = p.CreatedAt
    };
}
```

---

## 🎮 Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    public ProductController(IProductService service) => _service = service;

    // GET /api/products?search=laptop&page=1&pageSize=10&sortBy=name&sortDesc=false
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto query)
    {
        var result = await _service.GetAllAsync(query);
        return Ok(ApiResponse.Success(result, "Berhasil mengambil data produk"));
    }

    // GET /api/products/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null)
            return NotFound(ApiResponse.Fail($"Product dengan ID {id} tidak ditemukan"));
        return Ok(ApiResponse.Success(result));
    }

    // POST /api/products
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse.Success(result, "Product berhasil dibuat"));
    }

    // PUT /api/products/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);
        return Ok(ApiResponse.Success(result, "Product berhasil diupdate"));
    }

    // DELETE /api/products/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok(ApiResponse.Success<object?>(null, "Product berhasil dihapus"));
    }
}
```

---

## 📄 ApiResponse Wrapper

```csharp
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
}

public static class ApiResponse
{
    public static ApiResponse<T> Success<T>(T data, string message = "Success") => new()
    {
        Success = true,
        Message = message,
        Data    = data
    };

    public static ApiResponse<T> Fail<T>(string message, List<string>? errors = null) => new()
    {
        Success = false,
        Message = message,
        Errors  = errors
    };

    // Overload tanpa generic untuk kasus tidak perlu return data
    public static ApiResponse<object?> Fail(string message) => Fail<object?>(message);
}
```

---

## 📝 PagedResult

```csharp
public class PagedResult<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```

---

## 🔧 Program.cs (Setup)

```csharp
var builder = WebApplication.CreateBuilder(args);

// ── Database ──
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Dependency Injection ──
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// ── Controllers + Swagger ──
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## ⚙️ appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=TesBackendNet_CRUD;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 🔍 Query DTO

```csharp
public class ProductQueryDto
{
    public string? Search { get; set; }
    public string? SortBy { get; set; }        // "name", "price"
    public bool SortDesc { get; set; } = false;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
```
