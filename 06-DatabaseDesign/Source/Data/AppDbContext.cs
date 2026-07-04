// ============================================================
// AppDbContext.cs — DbContext Database Design
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.DatabaseDesign.Models;

namespace TesBackendNet.DatabaseDesign.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Scan assembly ini untuk mengaktifkan seluruh Entity Configurations otomatis
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Seed data awal
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "habibie", Email = "habibie@gmail.com", Age = 25 },
            new User { Id = 2, Username = "budi", Email = "budi@gmail.com", Age = 30 }
        );

        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, UserId = 1, TotalAmount = 250000, Status = "Completed", OrderDate = DateTime.UtcNow.AddDays(-2) },
            new Order { Id = 2, UserId = 1, TotalAmount = 450000, Status = "Pending", OrderDate = DateTime.UtcNow }
        );
    }
}
