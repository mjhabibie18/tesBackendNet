// ============================================================
// AppDbContext.cs — Database Context & Configurations
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.Database.Models;

namespace TesBackendNet.Database.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Blog> Blogs => Set<Blog>();
    public DbSet<Post> Posts => Set<Post>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Konfigurasi Relasi One-to-Many via Fluent API ───────
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
            
            entity.HasOne(p => p.Blog)
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BlogId)
                .OnDelete(DeleteBehavior.Cascade); // Jika Blog didelete, hapus seluruh Post miliknya
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Title).IsRequired().HasMaxLength(150);
        });

        // ── Seed Data Awal ─────────────────────────────────────
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
