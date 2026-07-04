// ============================================================
// AppDbContext.cs — Database Context
// ============================================================
// DbContext adalah "jembatan" antara aplikasi C# dan database.
// Ini adalah class utama yang:
//   1. Mendefinisikan tabel-tabel yang ada (via DbSet<T>)
//   2. Mengkonfigurasi relasi antar tabel
//   3. Mengelola koneksi database
//   4. Menjalankan query via LINQ
//
// Mengapa inherit dari DbContext?
//   - DbContext adalah base class dari EF Core
//   - Berisi semua logika ORM (query, change tracking, dll)
//   - Kita hanya perlu mendefinisikan DbSet dan konfigurasi tambahan
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.CRUD.Models;

namespace TesBackendNet.CRUD.Data;

/// <summary>
/// Database context untuk modul CRUD.
/// Mengelola koneksi ke SQL Server dan mendefinisikan tabel-tabel.
/// </summary>
public class AppDbContext : DbContext
{
    // ── Constructor ───────────────────────────────────────────
    // Constructor ini menerima DbContextOptions dari DI Container.
    // DbContextOptions berisi:
    //   - Connection string ke database
    //   - Provider (SQL Server, SQLite, dll)
    //   - Setting lain (retry, timeout, dll)
    //
    // base(options) meneruskan options ke parent class (DbContext)
    // karena DbContext butuh options untuk bisa bekerja.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // Tidak perlu isi constructor ini
        // Semua konfigurasi sudah di DbContextOptions
    }

    // ── DbSet Properties ─────────────────────────────────────
    // DbSet<T> merepresentasikan tabel di database.
    // Melalui DbSet, kita bisa query, tambah, update, hapus data.
    //
    // Set<Product>() adalah cara modern (EF Core 6+) mendefinisikan DbSet
    // Alternatif lama: public DbSet<Product> Products { get; set; }
    //
    // Nama property (Products) = nama tabel di database
    // EF Core akan buat tabel bernama "Products" di SQL Server
    public DbSet<Product> Products => Set<Product>();

    // ── OnModelCreating ───────────────────────────────────────
    // Method ini dipanggil saat model (tabel) pertama kali dikonfigurasi.
    // Digunakan untuk:
    //   - Konfigurasi kolom (tipe data, panjang, required)
    //   - Konfigurasi relasi (one-to-many, many-to-many)
    //   - Global Query Filter (soft delete)
    //   - Seed data awal
    //   - Index
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Memanggil implementasi base (penting! jangan hapus)
        base.OnModelCreating(modelBuilder);

        // ── Konfigurasi Tabel Products ────────────────────────
        modelBuilder.Entity<Product>(entity =>
        {
            // ── Nama Tabel ────────────────────────────────────
            // Eksplisit tentukan nama tabel
            // Default: EF Core pakai nama DbSet property ("Products")
            entity.ToTable("Products");

            // ── Primary Key ───────────────────────────────────
            // Eksplisit tentukan PK (sebenarnya sudah otomatis dari konvensi "Id")
            entity.HasKey(p => p.Id);

            // ── Konfigurasi Kolom Name ────────────────────────
            entity.Property(p => p.Name)
                  .IsRequired()           // NOT NULL di database
                  .HasMaxLength(200)       // VARCHAR(200)
                  .HasColumnType("nvarchar(200)"); // Unicode support

            // ── Konfigurasi Kolom Description ─────────────────
            entity.Property(p => p.Description)
                  .HasMaxLength(2000)
                  .HasColumnType("nvarchar(2000)");

            // ── Konfigurasi Kolom Price ───────────────────────
            // decimal(18,2) = total 18 digit, 2 digit desimal
            // Contoh: 9999999999999999.99 (maks nilai)
            entity.Property(p => p.Price)
                  .HasPrecision(18, 2)
                  .HasColumnType("decimal(18,2)");

            // ── Konfigurasi Kolom Stock ───────────────────────
            entity.Property(p => p.Stock)
                  .HasDefaultValue(0);

            // ── Konfigurasi Kolom CreatedAt ───────────────────
            entity.Property(p => p.CreatedAt)
                  .HasDefaultValueSql("GETUTCDATE()"); // SQL Server function

            // ── Index ─────────────────────────────────────────
            // Index pada kolom Name untuk mempercepat pencarian
            entity.HasIndex(p => p.Name)
                  .HasDatabaseName("IX_Products_Name");

            // Index pada IsDeleted untuk mempercepat soft delete query
            entity.HasIndex(p => p.IsDeleted)
                  .HasDatabaseName("IX_Products_IsDeleted");

            // ── Global Query Filter (Soft Delete) ─────────────
            // PENTING! Ini membuat SEMUA query ke Products otomatis
            // menambahkan WHERE IsDeleted = 0
            //
            // Artinya: _context.Products.ToList() akan OTOMATIS
            // hanya return data yang belum dihapus (IsDeleted = false)
            //
            // Jika ingin query data yang sudah dihapus:
            // _context.Products.IgnoreQueryFilters().ToList()
            entity.HasQueryFilter(p => !p.IsDeleted);
        });

        // ── Seed Data ─────────────────────────────────────────
        // Data awal yang dimasukkan saat migration pertama kali
        // Berguna untuk testing dan demo
        //
        // CATATAN: Jika pakai HasQueryFilter, seed data TETAP masuk
        // karena filter hanya berlaku saat query, bukan insert
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id          = 1,
                Name        = "Laptop Gaming ASUS ROG",
                Description = "Laptop gaming dengan RTX 4070, RAM 16GB, SSD 512GB",
                Price       = 25_000_000,
                Stock       = 15,
                IsDeleted   = false,
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id          = 2,
                Name        = "Mouse Wireless Logitech MX Master 3",
                Description = "Mouse wireless ergonomis dengan scroll horizontal",
                Price       = 1_200_000,
                Stock       = 50,
                IsDeleted   = false,
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id          = 3,
                Name        = "Mechanical Keyboard Keychron K2",
                Description = "Keyboard mechanical compact 75% dengan hot-swappable switches",
                Price       = 850_000,
                Stock       = 30,
                IsDeleted   = false,
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id          = 4,
                Name        = "Monitor LG 27 inch 4K",
                Description = "Monitor 4K UHD, IPS panel, 60Hz, sRGB 99%",
                Price       = 5_500_000,
                Stock       = 8,
                IsDeleted   = false,
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Product
            {
                Id          = 5,
                Name        = "Headset Sony WH-1000XM5",
                Description = "Headset wireless dengan ANC terbaik, 30 jam battery",
                Price       = 4_200_000,
                Stock       = 20,
                IsDeleted   = false,
                CreatedAt   = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
