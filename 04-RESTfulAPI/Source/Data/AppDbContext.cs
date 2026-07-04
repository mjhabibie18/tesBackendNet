// ============================================================
// Nama File: AppDbContext.cs — Database Context
// Folder: 04-RESTfulAPI/Source/Data/
// ============================================================
// 1. PENJELASAN FOLDER (RESTfulAPI/Source/Data):
//    - Tujuan: Mengelola akses ke database dan konfigurasi data mapping menggunakan EF Core.
//    - Kapan Digunakan: Saat aplikasi memerlukan operasi Create, Read, Update, atau Delete pada entitas database.
//    - Hubungan: Digunakan oleh Controller/Repository untuk berinteraksi dengan database.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menjadi jembatan utama antara kode C# (Object) dan SQL Server Database (Relational).
//    - Mengapa Diperlukan: Entity Framework Core membutuhkan turunan DbContext untuk melacak perubahan entitas, melakukan query LINQ-to-SQL, dan menjalankan migrasi skema tabel.
//    - Hubungan File: Memetakan class Model (Product.cs) ke tabel Products dan dipanggil di Program.cs untuk setup koneksi.
//    - Jika Dihapus: Aplikasi tidak dapat terhubung atau melakukan operasi data apa pun ke database SQL Server.
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.RESTfulAPI.Models;

namespace TesBackendNet.RESTfulAPI.Data;

/// <summary>
/// TUJUAN CLASS:
/// Kelas database context utama yang merepresentasikan sesi kerja dengan database SQL Server.
/// 
/// ALASAN MENGGUNAKAN DbContext:
/// DbContext menerapkan Unit of Work dan Repository Pattern secara internal. Ia melacak perubahan (Change Tracking)
/// pada objek entitas sehingga kita bisa melakukan pembaruan data secara efisien dengan memanggil SaveChangesAsync().
/// 
/// LIFECYCLE:
/// Scoped. Instance baru dari AppDbContext dibuat untuk setiap HTTP Request yang masuk 
/// dan dibuang otomatis (disposed) saat request berakhir untuk mencegah kebocoran koneksi database.
/// 
/// DEPENDENCY:
/// - DbContextOptions: Konfigurasi tingkat aplikasi (connection string, provider database seperti SQL Server, retry policy).
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// CONSTRUCTOR:
    /// Menerima opsi konfigurasi database dan meneruskannya ke base class DbContext.
    /// </summary>
    /// <param name="options">Konfigurasi database (seperti Connection String dari appsettings.json).</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// FUNGSI PROPERTY:
    /// Merepresentasikan tabel 'Products' di database SQL Server.
    /// 
    /// ALASAN TIPE DATA (DbSet):
    /// DbSet<T> menyediakan API untuk melakukan kueri LINQ serta menyimpan operasi CRUD terhadap entitas Product.
    /// Menggunakan `Set<Product>()` (C# 9+) adalah cara modern untuk menghindari warning nullable reference type.
    /// </summary>
    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// FUNGSI METHOD:
    /// Mengonfigurasi skema database dan melakukan seed data awal (data pembuka).
    /// 
    /// PARAMETER:
    /// - ModelBuilder: Fluent API builder untuk menyusun struktur relasi database.
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Memanggil base OnModelCreating untuk menjaga integritas konfigurasi bawaan EF Core.
    /// 2. Menggunakan `modelBuilder.Entity<Product>().HasData(...)` untuk mendefinisikan "Seed Data".
    /// 3. Data awal ini akan secara otomatis diisikan ke database saat proses Migrasi dijalankan (`dotnet ef database update`).
    /// 
    /// BEST PRACTICE:
    /// Gunakan OnModelCreating untuk relasi database kompleks, indeks unik, serta seed data statis (seperti data master).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seeding Data Awal (Master Data / Test Data)
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop ASUS ROG", Description = "Laptop gaming core i7", Price = 15000000, Stock = 10 },
            new Product { Id = 2, Name = "Mouse Logitech G102", Description = "Mouse gaming berkabel", Price = 250000, Stock = 50 },
            new Product { Id = 3, Name = "Mechanical Keyboard", Description = "Keyboard mechanical rgb", Price = 750000, Stock = 20 }
        );
    }
}
