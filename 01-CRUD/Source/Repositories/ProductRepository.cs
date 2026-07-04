// ============================================================
// ProductRepository.cs — Implementasi Repository
// ============================================================
// Repository adalah class yang bertanggung jawab HANYA untuk
// mengakses database. Semua logika query ada di sini.
//
// Mengapa Repository Pattern?
//   - Memisahkan data access dari business logic
//   - DRY: query yang sama tidak perlu ditulis ulang
//   - Testable: bisa di-mock saat unit test
//   - Maintainable: ganti ORM cukup di repository
//
// Mengapa inject AppDbContext lewat constructor?
//   - Dependency Injection: tidak new DbContext() sendiri
//   - DbContext bersifat Scoped (satu instance per request)
//   - Konsisten: request yang sama pakai DbContext yang sama
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.CRUD.Data;
using TesBackendNet.CRUD.DTO;
using TesBackendNet.CRUD.Models;

namespace TesBackendNet.CRUD.Repositories;

/// <summary>
/// Implementasi IProductRepository menggunakan Entity Framework Core.
/// Semua operasi database Product ada di sini.
/// </summary>
public class ProductRepository : IProductRepository
{
    // ── Private Fields ────────────────────────────────────────
    // readonly = tidak bisa diubah setelah constructor
    // Ini memastikan _context tidak di-replace di method lain
    private readonly AppDbContext _context;

    // ── Constructor Injection ─────────────────────────────────
    // Constructor menerima AppDbContext dari DI Container.
    // DI Container sudah mendaftarkan AppDbContext di Program.cs:
    //   builder.Services.AddDbContext<AppDbContext>(...)
    //
    // Kenapa tidak new AppDbContext()?
    //   - Tidak tahu connection string
    //   - Tidak ikut lifecycle management DI
    //   - Sulit di-test
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── GetAll ─────────────────────────────────────────────────
    // Method paling kompleks: search + filter + sort + pagination
    //
    // Mengapa return (List<Product>, int)?
    //   - Tuple: mengembalikan dua nilai sekaligus
    //   - TotalCount diperlukan untuk kalkulasi pagination di Service
    //   - Lebih efisien dari dua query terpisah
    public async Task<(List<Product> Data, int TotalCount)> GetAllAsync(ProductQueryDto query)
    {
        // ── AsQueryable() ─────────────────────────────────────
        // AsQueryable() = buat query LINQ yang belum dieksekusi
        // Kita bisa terus menambahkan kondisi WHERE, ORDER BY, dll
        // sebelum benar-benar mengirim query ke database
        //
        // LINQ lazy: query baru dieksekusi saat ToListAsync() dipanggil
        // Ini lebih efisien karena hanya satu round-trip ke database
        var queryable = _context.Products.AsQueryable();
        // CATATAN: Global Query Filter sudah otomatis menambahkan
        // WHERE IsDeleted = 0, jadi kita tidak perlu tambahkan lagi

        // ── Search ────────────────────────────────────────────
        // Cari berdasarkan keyword di Name atau Description
        //
        // !string.IsNullOrWhiteSpace() = cek string tidak null, kosong, atau spasi saja
        // Tanpa pengecekan ini, Contains("") akan match semua data
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var searchLower = query.Search.ToLower().Trim();

            // EF Core menerjemahkan Contains() menjadi SQL LIKE '%keyword%'
            // ToLower() untuk case-insensitive search
            queryable = queryable.Where(p =>
                p.Name.ToLower().Contains(searchLower) ||
                (p.Description != null && p.Description.ToLower().Contains(searchLower)));
        }

        // ── Filter Harga ──────────────────────────────────────
        if (query.MinPrice.HasValue)
            queryable = queryable.Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            queryable = queryable.Where(p => p.Price <= query.MaxPrice.Value);

        // ── Count Total ───────────────────────────────────────
        // Hitung total SEBELUM apply pagination
        // CountAsync() = SELECT COUNT(*) FROM ... dengan WHERE yang sudah ada
        // Ini adalah query pertama ke database
        var totalCount = await queryable.CountAsync();

        // ── Sorting ───────────────────────────────────────────
        // switch expression (C# 8+): lebih ringkas dari if-else
        // Pattern: sortBy?.ToLower() → match case-insensitive
        //
        // Mengapa harus sort sebelum pagination?
        //   - Pagination tanpa sorting tidak deterministic
        //   - Data bisa berbeda setiap request jika tidak di-sort
        queryable = query.SortBy?.ToLower() switch
        {
            "name"      => query.SortDesc
                           ? queryable.OrderByDescending(p => p.Name)
                           : queryable.OrderBy(p => p.Name),
            "price"     => query.SortDesc
                           ? queryable.OrderByDescending(p => p.Price)
                           : queryable.OrderBy(p => p.Price),
            "stock"     => query.SortDesc
                           ? queryable.OrderByDescending(p => p.Stock)
                           : queryable.OrderBy(p => p.Stock),
            "createdat" => query.SortDesc
                           ? queryable.OrderByDescending(p => p.CreatedAt)
                           : queryable.OrderBy(p => p.CreatedAt),
            _           => queryable.OrderByDescending(p => p.CreatedAt) // default: terbaru dulu
        };

        // ── Pagination ────────────────────────────────────────
        // Skip() = SQL OFFSET: lewati N data pertama
        // Take() = SQL FETCH NEXT: ambil N data berikutnya
        //
        // Formula OFFSET: (page - 1) * pageSize
        //   Page 1: (1-1) * 10 = 0    → ambil 10 data pertama
        //   Page 2: (2-1) * 10 = 10   → lewati 10, ambil 10 berikutnya
        //   Page 3: (3-1) * 10 = 20   → lewati 20, ambil 10 berikutnya
        //
        // ToListAsync() = EKSEKUSI QUERY ke database (query kedua)
        var data = await queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return (data, totalCount);
    }

    // ── GetById ────────────────────────────────────────────────
    public async Task<Product?> GetByIdAsync(int id)
    {
        // FindAsync(): mencari berdasarkan Primary Key
        // Lebih efisien dari FirstOrDefaultAsync karena:
        //   1. Cek cache (first-level cache / identity map) dulu
        //   2. Jika ada di cache, tidak perlu ke database
        //   3. Jika tidak ada, baru query database
        //
        // Return null jika tidak ditemukan ATAU sudah soft deleted
        // (karena Global Query Filter aktif)
        return await _context.Products.FindAsync(id);
    }

    // ── ExistsByName ───────────────────────────────────────────
    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        // AnyAsync(): SELECT TOP 1 / EXISTS — lebih efisien dari CountAsync
        // Begitu menemukan satu data yang match, langsung return true
        // Tidak perlu hitung semua data
        return await _context.Products.AnyAsync(p =>
            p.Name.ToLower() == name.ToLower() &&
            (!excludeId.HasValue || p.Id != excludeId.Value));
        // excludeId: saat update, kecualikan product itu sendiri dari pengecekan
        // Contoh: update Product Id=1 dengan nama "Laptop"
        //         → cek apakah ada product LAIN dengan nama "Laptop"
        //         → excludeId = 1, sehingga Product Id=1 tidak dianggap duplikat
    }

    // ── Add ───────────────────────────────────────────────────
    public async Task AddAsync(Product product)
    {
        // AddAsync(): menambahkan entity ke DbContext (bukan ke database)
        // EF Core track entity ini dengan state "Added"
        await _context.Products.AddAsync(product);

        // SaveChangesAsync(): BARU kirim ke database
        // SQL: INSERT INTO Products (Name, Price, ...) VALUES (...)
        //
        // Setelah SaveChanges, EF Core update:
        //   - product.Id = ID yang di-generate database
        //   - State entity menjadi "Unchanged"
        await _context.SaveChangesAsync();
    }

    // ── Update ────────────────────────────────────────────────
    public async Task UpdateAsync(Product product)
    {
        // Update(): tandai entity sebagai "Modified"
        // EF Core akan track semua perubahan dan generate SQL UPDATE
        //
        // Berbeda dari Add():
        //   - Add: entity baru, state = Added
        //   - Update: entity lama, state = Modified
        _context.Products.Update(product);

        // SQL: UPDATE Products SET Name=..., Price=... WHERE Id=...
        await _context.SaveChangesAsync();
    }

    // ── SoftDelete ────────────────────────────────────────────
    public async Task SoftDeleteAsync(int id)
    {
        // Cari product (IgnoreQueryFilters untuk bypass soft delete filter)
        // Kenapa IgnoreQueryFilters? Karena:
        //   - Global Query Filter: WHERE IsDeleted = 0
        //   - Jika product sudah IsDeleted = true, FindAsync akan return null
        //   - Tapi kita butuh product itu untuk update IsDeleted
        //
        // Sebenarnya FindAsync juga bisa dipakai karena:
        //   - Saat soft delete, product belum di-mark sebagai deleted
        //   - Sehingga Global Filter tidak memblok
        var product = await _context.Products.FindAsync(id);
        if (product == null) return;

        // Tandai sebagai deleted (BUKAN hapus dari database!)
        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        product.DeletedAt = DateTime.UtcNow;

        // SQL: UPDATE Products SET IsDeleted=1, UpdatedAt=..., DeletedAt=... WHERE Id=...
        await _context.SaveChangesAsync();
    }
}
