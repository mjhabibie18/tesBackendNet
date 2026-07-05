// ============================================================
// Nama File: AppDbContext.cs — Database Context (05-Database)
// Folder: 05-Database/Source/Data/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendaftarkan entitas Blog dan Post serta mendefinisikan relasi antar keduanya menggunakan Fluent API.
//    - Mengapa Diperlukan: EF Core menggunakan DbContext untuk melacak perubahan entitas, menerjemahkan LINQ ke SQL, dan menerapkan constraint relasional.
//    - Hubungan File: Memanfaatkan Blog.cs dan Post.cs sebagai skema tabel, dipanggil di Program.cs melalui AddDbContext.
//    - Jika Dihapus: Seluruh akses database tidak dapat berjalan.
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.Database.Models;

namespace TesBackendNet.Database.Data;

/// <summary>
/// TUJUAN CLASS:
/// Kelas database session utama untuk modul 05-Database yang mengelola dua tabel: Blogs dan Posts.
/// 
/// CARA KERJA FLUENT API:
/// EF Core mendukung dua cara konfigurasi model:
/// 1. Data Annotations (atribut langsung di model class, misal [Required], [MaxLength]).
/// 2. Fluent API (di dalam OnModelCreating menggunakan method chaining) — lebih powerful dan tidak mencemari model class.
/// Modul ini menggunakan Fluent API sepenuhnya sebagai demonstrasi gaya penulisan yang direkomendasikan.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// CONSTRUCTOR: Menerima opsi koneksi database yang disuntikkan dari DI Container.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// FUNGSI PROPERTY: Memetakan entitas Blog ke tabel 'Blogs' di database SQL Server.
    /// </summary>
    public DbSet<Blog> Blogs => Set<Blog>();

    /// <summary>
    /// FUNGSI PROPERTY: Memetakan entitas Post ke tabel 'Posts' di database SQL Server.
    /// </summary>
    public DbSet<Post> Posts => Set<Post>();

    /// <summary>
    /// FUNGSI METHOD: Mengonfigurasi skema database, relasi antar entitas, constraint, dan seed data.
    /// 
    /// KONFIGURASI RELASI ONE-TO-MANY (HasOne - WithMany):
    /// - `HasOne(p => p.Blog)`: Satu Post memiliki satu referensi Blog (sisi "banyak").
    /// - `.WithMany(b => b.Posts)`: Satu Blog memiliki banyak Post (sisi "satu").
    /// - `.HasForeignKey(p => p.BlogId)`: Kolom yang menjadi kunci relasi di tabel Posts.
    /// - `.OnDelete(DeleteBehavior.Cascade)`: Saat satu baris Blog dihapus, SQL Server secara otomatis menghapus
    ///   semua baris Post yang merujuk ke Blog tersebut. Ini menjaga integritas referensial database.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Konfigurasi Relasi One-to-Many via Fluent API ───────
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
            
            entity.HasOne(p => p.Blog)       // Post memiliki satu Blog sebagai induk
                .WithMany(b => b.Posts)       // Blog memiliki banyak Post sebagai turunan
                .HasForeignKey(p => p.BlogId) // BlogId di tabel Posts adalah Foreign Key
                .OnDelete(DeleteBehavior.Cascade); // Hapus Blog = hapus semua Post miliknya
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(150);
        });

        // ── Seed Data Awal ─────────────────────────────────────
        // Data ini diinsert secara otomatis saat proses migrasi dijalankan.
        // Berguna untuk ketersediaan data uji coba tanpa perlu input manual.
        modelBuilder.Entity<Blog>().HasData(
            new Blog { Id = 1, Title = "Tech Blog", Url = "https://techblog.com" },
            new Blog { Id = 2, Title = "Lifestyle Blog", Url = "https://lifestyleblog.com" }
        );

        modelBuilder.Entity<Post>().HasData(
            new Post { Id = 1, Title = "Belajar ASP.NET Core", Content = "Tutorial dasar ASP.NET Core...", Views = 1500, BlogId = 1 },
            new Post { Id = 2, Title = "Mengenal Entity Framework", Content = "Tutorial setup DbContext...", Views = 850, BlogId = 1 },
            new Post { Id = 3, Title = "Tips Hidup Sehat", Content = "Minum air putih minimal 2L...", Views = 340, BlogId = 2 },
            new Post { Id = 4, Title = "Review Sepatu Lari", Content = "Review sepatu lari merk X...", Views = 210, BlogId = 2 }
        );
    }
}
