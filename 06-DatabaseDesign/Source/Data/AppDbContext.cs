// ============================================================
// Nama File: AppDbContext.cs — DbContext dengan ApplyConfigurationsFromAssembly
// Folder: 06-DatabaseDesign/Source/Data/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendaftarkan entitas User dan Order, serta menerapkan seluruh konfigurasi Fluent API
//      secara otomatis dari assembly melalui ApplyConfigurationsFromAssembly.
//    - Mengapa Diperlukan: Pola ini memisahkan konfigurasi setiap entitas ke file IEntityTypeConfiguration<T> tersendiri,
//      sehingga DbContext tidak "bengkak" dengan ratusan baris konfigurasi.
//    - Hubungan File: Memanfaatkan semua class yang mengimplementasikan IEntityTypeConfiguration<T> di folder Data/Configurations.
//    - Jika Dihapus: Seluruh akses database tidak dapat dilakukan.
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.DatabaseDesign.Models;

namespace TesBackendNet.DatabaseDesign.Data;

/// <summary>
/// TUJUAN CLASS:
/// Database session context yang menerapkan pola konfigurasi terpisah per entitas menggunakan
/// `ApplyConfigurationsFromAssembly` untuk menjaga DbContext tetap bersih dan modular.
/// 
/// POLA KONFIGURASI: IEntityTypeConfiguration<T>
/// Daripada menulis semua konfigurasi Fluent API langsung di dalam `OnModelCreating()`,
/// setiap entitas memiliki file konfigurasi tersendiri (misal: UserConfiguration.cs, OrderConfiguration.cs)
/// yang mengimplementasikan `IEntityTypeConfiguration<T>`.
/// `ApplyConfigurationsFromAssembly()` secara otomatis menemukan dan menerapkan semua kelas tersebut.
/// 
/// KEUNTUNGAN POLA INI:
/// - DbContext tetap singkat dan mudah dibaca.
/// - Setiap konfigurasi entitas terisolasi dan mudah diubah.
/// - Sesuai dengan prinsip Single Responsibility Principle (SRP).
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// CONSTRUCTOR: Menerima opsi koneksi database dari DI Container.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// FUNGSI PROPERTY: Memetakan entitas User ke tabel 'Users' di SQL Server.
    /// </summary>
    public DbSet<User>  Users  => Set<User>();

    /// <summary>
    /// FUNGSI PROPERTY: Memetakan entitas Order ke tabel 'Orders' di SQL Server.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>
    /// FUNGSI METHOD: Mengonfigurasi model dan menerapkan semua konfigurasi entitas dari assembly ini.
    /// 
    /// BARIS KODE PENTING:
    /// `modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly)`:
    /// - `typeof(AppDbContext).Assembly`: Mengambil referensi assembly (.dll) dari proyek ini saat runtime.
    /// - EF Core mencari semua kelas yang mengimplementasikan `IEntityTypeConfiguration<T>` di dalam assembly.
    /// - Setiap kelas konfigurasi yang ditemukan otomatis dipanggil — tidak perlu mendaftarkan satu per satu.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Pindai seluruh assembly proyek ini untuk menerapkan semua konfigurasi entitas secara otomatis
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── Seed Data Awal ─────────────────────────────────────
        // Data uji coba yang tersedia segera setelah migrasi pertama dijalankan
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "habibie", Email = "habibie@gmail.com", Age = 25 },
            new User { Id = 2, Username = "budi",    Email = "budi@gmail.com",    Age = 30 }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, UserId = 1, TotalAmount = 250000, Status = "Completed", OrderDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Order { Id = 2, UserId = 1, TotalAmount = 450000, Status = "Pending",   OrderDate = new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
