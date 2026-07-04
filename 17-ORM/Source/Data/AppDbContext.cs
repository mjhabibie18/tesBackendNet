// ============================================================
// AppDbContext.cs — Database Context untuk ORM
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.ORM.Models;

namespace TesBackendNet.ORM.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Mengaplikasikan Semua Konfigurasi Otomatis ──────────
        // Menggunakan refleksi untuk memindai assembly ini dan mengaktifkan
        // konfigurasi yang mengimplementasikan IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // ── Seed Data Awal ─────────────────────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Elektronik" },
            new Category { Id = 2, Name = "Pakaian" }
        );

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop ASUS", Price = 12000000, Stock = 10, CategoryId = 1 },
            new Product { Id = 2, Name = "Keyboard Mechanical", Price = 800000, Stock = 25, CategoryId = 1 },
            new Product { Id = 3, Name = "Kaos Polos Cotton", Price = 75000, Stock = 100, CategoryId = 2 }
        );
    }
}
