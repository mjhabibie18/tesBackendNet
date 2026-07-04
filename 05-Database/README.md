# 🗄️ 05 — Database & Entity Framework Core

## Apa itu ORM?

**ORM** (Object-Relational Mapper) adalah tool yang memetakan tabel database ke class C# dan sebaliknya, sehingga kita tidak perlu menulis SQL secara manual.

**Entity Framework Core** adalah ORM resmi dari Microsoft untuk .NET.

---

## Mengapa EF Core?

```
✓ Write C# instead of SQL
✓ Type-safe queries (LINQ)
✓ Database migrations
✓ Change tracking otomatis
✓ Support banyak database (SQL Server, PostgreSQL, SQLite, MySQL)
✓ Lazy/Eager loading
```

---

## Database-First vs Code-First

| Pendekatan | Deskripsi | Kapan |
|------------|-----------|-------|
| **Code-First** | Buat class C# dulu, generate database | Project baru |
| **Database-First** | Database sudah ada, generate class | Legacy database |

> Repository ini menggunakan **Code-First** approach.

---

## Migrations

```bash
# Buat migration baru
dotnet ef migrations add NamaMigration

# Apply migration ke database
dotnet ef database update

# Rollback ke migration sebelumnya
dotnet ef database update NamaMigrationSebelumnya

# Hapus migration terakhir (yang belum di-apply)
dotnet ef migrations remove

# Lihat semua migration
dotnet ef migrations list

# Generate SQL script
dotnet ef migrations script
```

---

## LINQ to SQL

```csharp
// SELECT * FROM Products WHERE IsDeleted = 0
var products = await _context.Products
    .Where(p => !p.IsDeleted)
    .ToListAsync();

// SELECT * FROM Products WHERE Price > 100000 ORDER BY Price
var expensive = await _context.Products
    .Where(p => p.Price > 100000)
    .OrderByDescending(p => p.Price)
    .ToListAsync();

// JOIN
var productWithCategory = await _context.Products
    .Include(p => p.Category)
    .Where(p => p.CategoryId == 1)
    .ToListAsync();

// GROUP BY
var countByCategory = await _context.Products
    .GroupBy(p => p.CategoryId)
    .Select(g => new { CategoryId = g.Key, Count = g.Count() })
    .ToListAsync();

// Aggregate
var totalStock  = await _context.Products.SumAsync(p => p.Stock);
var averagePrice = await _context.Products.AverageAsync(p => p.Price);
var maxPrice    = await _context.Products.MaxAsync(p => p.Price);
```

---

## Relasi

```csharp
// One-to-Many
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = new(); // Navigation
}

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }          // Foreign Key
    public Category Category { get; set; } = null!; // Navigation
}

// Di DbContext
modelBuilder.Entity<Product>()
    .HasOne(p => p.Category)
    .WithMany(c => c.Products)
    .HasForeignKey(p => p.CategoryId);

// Many-to-Many
// EF Core 5+ otomatis handle junction table
public class Product
{
    public List<Tag> Tags { get; set; } = new();
}

public class Tag
{
    public List<Product> Products { get; set; } = new();
}
```

---

## Transaction

```csharp
// Transaction manual
await using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    _context.Orders.Add(order);
    _context.Inventory.Update(inventory); // kurangi stok
    await _context.SaveChangesAsync();

    await transaction.CommitAsync(); // Commit jika semua berhasil
}
catch
{
    await transaction.RollbackAsync(); // Rollback jika ada error
    throw;
}
```

---

## 🎤 Tips Interview

**Q: "Apa itu N+1 Problem?"**
```
Query yang menghasilkan N query tambahan untuk setiap item.

Contoh:
// ❌ N+1: 1 query ambil products + N query untuk setiap category
var products = await _context.Products.ToListAsync();
foreach (var p in products)
    Console.WriteLine(p.Category.Name); // Lazy load = N query!

// ✅ Eager loading: hanya 1 query
var products = await _context.Products
    .Include(p => p.Category) // JOIN
    .ToListAsync();
```

**Q: "Kapan pakai AsNoTracking?"**
```
AsNoTracking() = EF Core tidak track perubahan entity
Gunakan untuk query READ-ONLY agar lebih cepat dan hemat memory

var products = await _context.Products
    .AsNoTracking()  // ← tidak di-track
    .ToListAsync();
```
