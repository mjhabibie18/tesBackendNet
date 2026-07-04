# 🗄️ 02 — Database Interview Questions

## SQL, EF Core, Database Design

---

## Soal 1: Apa bedanya INNER JOIN, LEFT JOIN, RIGHT JOIN, FULL JOIN?

**Tingkat**: Easy | **Topik**: SQL

### Kode Lengkap

```sql
-- Data:
-- Products: { 1:'Laptop', catId:1 }, { 2:'Mouse', catId:null }
-- Categories: { 1:'Electronics' }, { 2:'Furniture' }

-- INNER JOIN: hanya data yang match di KEDUA tabel
SELECT p.Name, c.Name
FROM Products p
INNER JOIN Categories c ON p.CategoryId = c.Id
-- Result: Laptop, Electronics (Mouse tidak muncul karena catId null)

-- LEFT JOIN: semua dari kiri + match kanan (null jika tidak ada)
SELECT p.Name, c.Name
FROM Products p
LEFT JOIN Categories c ON p.CategoryId = c.Id
-- Result: Laptop, Electronics | Mouse, NULL

-- RIGHT JOIN: semua dari kanan + match kiri
SELECT p.Name, c.Name
FROM Products p
RIGHT JOIN Categories c ON p.CategoryId = c.Id
-- Result: Laptop, Electronics | NULL, Furniture

-- FULL OUTER JOIN: semua dari kedua tabel
SELECT p.Name, c.Name
FROM Products p
FULL OUTER JOIN Categories c ON p.CategoryId = c.Id
-- Result: Laptop, Electronics | Mouse, NULL | NULL, Furniture
```

---

## Soal 2: Jelaskan Index dan kapan menggunakannya!

**Tingkat**: Medium | **Topik**: Database Design, Performance

### Solusi

```sql
-- Tanpa index: full table scan O(n)
SELECT * FROM Products WHERE Name = 'Laptop'; -- Scan semua row!

-- Dengan index: O(log n)
CREATE INDEX IX_Products_Name ON Products(Name);
SELECT * FROM Products WHERE Name = 'Laptop'; -- Langsung ke row yang tepat!

-- Clustered vs Non-Clustered
-- Clustered: urutan fisik data (otomatis untuk PK)
-- Non-Clustered: struktur terpisah (index → row pointer)

-- Composite Index: urutan kolom PENTING
CREATE INDEX IX_Orders_Status_Date ON Orders(Status, CreatedAt);
-- Efektif untuk: WHERE Status = 'Pending' AND CreatedAt > '2024-01-01'
-- Tidak efektif untuk: WHERE CreatedAt > '2024-01-01' (tanpa Status)

-- Kapan TIDAK pakai index:
-- Tabel kecil (< 1000 row)
-- Kolom yang sering di-UPDATE (index perlu di-update juga)
-- Kolom dengan sedikit variasi (IsDeleted: hanya 0 atau 1)
```

---

## Soal 3: Apa itu Transaction dan kapan digunakan?

**Tingkat**: Medium | **Topik**: SQL, EF Core

### Kode Lengkap

```csharp
// Transaction: kumpulan operasi yang harus SEMUA berhasil atau SEMUA gagal
// Properti ACID:
//   Atomicity:   semua atau tidak sama sekali
//   Consistency: database selalu valid sebelum dan sesudah
//   Isolation:   transaksi tidak interfere satu sama lain
//   Durability:  setelah commit, data tersimpan permanent

// EF Core Transaction
public async Task TransferStockAsync(int fromProductId, int toProductId, int quantity)
{
    await using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        var fromProduct = await _context.Products.FindAsync(fromProductId);
        var toProduct   = await _context.Products.FindAsync(toProductId);

        if (fromProduct!.Stock < quantity)
            throw new InvalidOperationException("Stok tidak cukup");

        fromProduct.Stock -= quantity;
        toProduct!.Stock  += quantity;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}
```

---

## Soal 4: Tulis query untuk laporan penjualan per bulan!

**Tingkat**: Medium | **Topik**: SQL Aggregate

### Kode Lengkap

```sql
-- Orders: { Id, CustomerId, TotalAmount, CreatedAt, Status }

-- Laporan penjualan per bulan
SELECT
    YEAR(CreatedAt)  AS Tahun,
    MONTH(CreatedAt) AS Bulan,
    DATENAME(MONTH, CreatedAt) AS NamaBulan,
    COUNT(*)         AS TotalOrder,
    SUM(TotalAmount) AS TotalPendapatan,
    AVG(TotalAmount) AS RataRataOrder,
    MIN(TotalAmount) AS OrderTerkecil,
    MAX(TotalAmount) AS OrderTerbesar
FROM Orders
WHERE Status = 'Completed'
  AND CreatedAt >= '2024-01-01'
GROUP BY
    YEAR(CreatedAt),
    MONTH(CreatedAt),
    DATENAME(MONTH, CreatedAt)
ORDER BY
    Tahun, Bulan;
```

---

## Soal 5: Jelaskan N+1 Problem dan solusinya di EF Core!

**Tingkat**: Medium | **Topik**: EF Core, Performance

### Kode Lengkap

```csharp
// ❌ N+1 Problem
var products = await _context.Products.ToListAsync(); // Query 1: SELECT * FROM Products

foreach (var product in products) // N query berikutnya!
{
    // Setiap akses Category = satu query baru (lazy load)
    Console.WriteLine(product.Category.Name); // Query 2,3,4,...N
}
// Total: 1 + N queries = N+1 problem!

// ✅ Solusi: Eager Loading
var products = await _context.Products
    .Include(p => p.Category)   // JOIN Categories dalam SATU query
    .ToListAsync();

foreach (var product in products)
    Console.WriteLine(product.Category.Name); // Tidak perlu query tambahan!

// ✅ Solusi: Select (ambil hanya yang perlu)
var products = await _context.Products
    .Select(p => new
    {
        p.Name,
        CategoryName = p.Category.Name // EF Core otomatis JOIN
    })
    .ToListAsync();
```

---

## Soal 6: Apa itu Normalization? Jelaskan 1NF, 2NF, 3NF!

**Tingkat**: Medium | **Topik**: Database Design

### Solusi

```
1NF (First Normal Form):
  - Setiap sel hanya satu nilai (atomic)
  - Tidak ada grup yang berulang
  
  ❌ Tidak 1NF: Products(Id, Name, Tags="electronics,gaming,laptop")
  ✅ 1NF: ProductTags(ProductId, Tag)

2NF (Second Normal Form):
  - Sudah 1NF
  - Tidak ada partial dependency pada composite PK
  
  ❌ Tidak 2NF: OrderItems(OrderId, ProductId, ProductName, Qty)
     ProductName bergantung pada ProductId saja (bukan OrderId+ProductId)
  ✅ 2NF: Pisahkan ke Products(ProductId, ProductName) dan OrderItems(OrderId, ProductId, Qty)

3NF (Third Normal Form):
  - Sudah 2NF
  - Tidak ada transitive dependency
  
  ❌ Tidak 3NF: Employees(Id, Name, DeptId, DeptName)
     DeptName bergantung pada DeptId, bukan Id langsung
  ✅ 3NF: Departments(DeptId, DeptName) dan Employees(Id, Name, DeptId)
```

---

## Soal 7: Tulis stored procedure untuk CRUD dengan EF Core!

**Tingkat**: Hard | **Topik**: SQL, EF Core

### Kode Lengkap

```sql
-- Stored Procedure
CREATE PROCEDURE sp_GetProductsPaginated
    @Search    NVARCHAR(200) = NULL,
    @Page      INT           = 1,
    @PageSize  INT           = 10
AS
BEGIN
    SELECT *, COUNT(*) OVER() AS TotalCount
    FROM Products
    WHERE IsDeleted = 0
      AND (@Search IS NULL OR Name LIKE '%' + @Search + '%')
    ORDER BY CreatedAt DESC
    OFFSET (@Page - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
```

```csharp
// Panggil dari EF Core
var result = await _context.Products
    .FromSqlInterpolated(
        $"EXEC sp_GetProductsPaginated @Search={search}, @Page={page}, @PageSize={pageSize}")
    .ToListAsync();
```

---

## Soal 8–25: (Ringkasan)

**8.** Apa itu Optimistic vs Pessimistic Locking?
> Optimistic: cek konflik saat save (rowversion/timestamp)
> Pessimistic: lock row saat read (SELECT ... WITH (UPDLOCK))

**9.** Bagaimana cara handle soft delete dengan EF Core?
> HasQueryFilter(e => !e.IsDeleted) — otomatis filter semua query

**10.** Apa bedanya `SaveChanges` dan `SaveChangesAsync`?
> Async: tidak blokir thread, recommended untuk web API

**11.** Apa itu DbContext lifetime dan mengapa Scoped?
> Scoped = satu instance per request. Singleton berbahaya (thread safety, connection leak)

**12.** Bagaimana cara seed data di EF Core?
> OnModelCreating: `modelBuilder.Entity<T>().HasData(...)` + migration

**13.** Apa itu Migration dan bagaimana cara rollback?
> `dotnet ef database update MigrationName` untuk rollback ke migration tertentu

**14.** Apa itu Composite Key di EF Core?
> `HasKey(e => new { e.ProductId, e.TagId })` untuk many-to-many junction table

**15.** Bagaimana cara query dengan GROUP BY di LINQ?
> `.GroupBy(e => e.CategoryId).Select(g => new { g.Key, Count = g.Count() })`

**16.** Apa itu Computed Column di EF Core?
> `.HasComputedColumnSql("Price * Stock")` — dihitung di database

**17.** Bagaimana cara prevent SQL Injection?
> EF Core otomatis parameterized. Raw SQL: `FromSqlInterpolated` ($"...{param}...")

**18.** Apa bedanya DELETE dan TRUNCATE?
> DELETE: logged, bisa WHERE, trigger, rollback. TRUNCATE: tidak logged, hapus semua, lebih cepat

**19.** Apa itu Covering Index?
> Index yang berisi semua kolom yang diperlukan query → tidak perlu lookup ke tabel utama

**20.** Apa itu CTE (Common Table Expression)?
> Temporary named result set untuk query yang lebih readable dan recursive

```sql
WITH SalesCTE AS (
    SELECT UserId, SUM(Amount) AS Total
    FROM Sales
    GROUP BY UserId
)
SELECT u.Name, s.Total
FROM Users u
JOIN SalesCTE s ON u.Id = s.UserId;
```

**21.** Jelaskan Window Functions!
> Fungsi yang beroperasi pada set row terkait tanpa GROUP BY:
> `ROW_NUMBER() OVER(ORDER BY Price)`, `RANK()`, `LAG()`, `LEAD()`

**22.** Apa itu Execution Plan?
> Visual/text representasi bagaimana SQL Server menjalankan query. Gunakan: `EXPLAIN` atau `SET STATISTICS IO ON`

**23.** Bagaimana cara debug query yang lambat?
> 1. Check execution plan, 2. Check index usage, 3. EF Core SQL logging, 4. SQL Profiler

**24.** Apa itu Connection Pooling?
> Pool koneksi database yang di-reuse → tidak perlu open/close setiap request (expensive!)
> EF Core dan SQL Server otomatis handle connection pooling.

**25.** Apa itu Database Sharding?
> Membagi data ke beberapa database (shard) berdasarkan key
> Contoh: User ID 1-1000 → DB1, 1001-2000 → DB2
