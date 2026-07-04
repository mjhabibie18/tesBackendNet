# 🗃️ 17 — ORM (Entity Framework Core)

## EF Core Deep Dive

Lihat juga: [05-Database](../05-Database/) untuk dasar EF Core.

---

## Fluent API vs Data Annotations

```csharp
// Data Annotations (di Model)
public class Product
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
}

// Fluent API (di DbContext) — lebih powerful dan recommended
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.Entity<Product>(e =>
    {
        e.Property(p => p.Name).IsRequired().HasMaxLength(200);
        e.Property(p => p.Price).HasPrecision(18, 2);
        e.HasIndex(p => p.Name).IsUnique();
    });
}
```

---

## IEntityTypeConfiguration (Best Practice)

Pisahkan konfigurasi per entity ke file terpisah:

```csharp
// ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.HasIndex(p => p.Name);
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}

// DbContext — apply semua configuration otomatis
protected override void OnModelCreating(ModelBuilder builder)
{
    builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
```

---

## Change Tracking

```csharp
// EF Core track semua entity yang di-load dari database
var product = await _context.Products.FindAsync(id); // → state: Unchanged
product.Name = "New Name"; // → state: Modified (otomatis!)
await _context.SaveChangesAsync(); // → UPDATE Products SET Name=... WHERE Id=...

// Disable tracking untuk query READ-ONLY (lebih cepat)
var product = await _context.Products
    .AsNoTracking()
    .FirstOrDefaultAsync(p => p.Id == id);
// product.Name = "New Name"; // Tidak akan ter-track!

// Explicit tracking
_context.Entry(product).State = EntityState.Modified;
```

---

## Eager, Lazy, Explicit Loading

```csharp
// Eager Loading: JOIN dalam satu query
var product = await _context.Products
    .Include(p => p.Category)           // JOIN Categories
    .Include(p => p.Tags)               // JOIN Tags
    .ThenInclude(t => t.Color)          // JOIN Colors (nested)
    .FirstOrDefaultAsync(p => p.Id == id);

// Lazy Loading: otomatis query saat diakses (perlu setup virtual)
// HATI-HATI: N+1 problem!
services.AddDbContext<AppDb>(o => o.UseLazyLoadingProxies());
public virtual Category Category { get; set; } // harus virtual

// Explicit Loading: manual load navigasi
var product = await _context.Products.FindAsync(id);
await _context.Entry(product).Reference(p => p.Category).LoadAsync();
await _context.Entry(product).Collection(p => p.Reviews).LoadAsync();
```

---

## Raw SQL

```csharp
// Gunakan saat LINQ tidak cukup atau perlu performance
var products = await _context.Products
    .FromSqlRaw("SELECT * FROM Products WHERE Price > {0}", 100000)
    .ToListAsync();

// Dengan interpolation (AMAN dari injection)
var minPrice = 100000;
var products = await _context.Products
    .FromSqlInterpolated($"SELECT * FROM Products WHERE Price > {minPrice}")
    .ToListAsync();

// Non-query (INSERT, UPDATE, DELETE)
var rowsAffected = await _context.Database
    .ExecuteSqlInterpolatedAsync(
        $"UPDATE Products SET IsActive = 0 WHERE ExpiredAt < {DateTime.UtcNow}");
```

---

## 🎤 Tips Interview

**Q: "Apa itu N+1 Problem dan bagaimana solusinya?"**
```
N+1: satu query untuk list, N query untuk setiap item (navigasi)
Solusi: gunakan Include() untuk eager loading
```

**Q: "Kapan gunakan AsNoTracking?"**
```
Untuk query READ-ONLY yang tidak perlu update/delete.
Lebih cepat karena EF Core tidak perlu track state perubahan.
```
