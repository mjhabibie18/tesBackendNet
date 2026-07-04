// ============================================================
// AppDbContext.cs — Database Context
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.Authorization.Models;

namespace TesBackendNet.Authorization.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfigurasi User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(150);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
        });

        // Seed Data Admin Default
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@example.com",
                // Password: "Password123!" dihash dengan bcrypt work factor 11
                PasswordHash = "$2a$11$eE6M15vG./4J3y11.w77HekvR/c.26l.7nC4XvK8W2/fT44/iZ6Wq",
                FirstName = "Super",
                LastName = "Admin",
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 2,
                Email = "manager@example.com",
                PasswordHash = "$2a$11$eE6M15vG./4J3y11.w77HekvR/c.26l.7nC4XvK8W2/fT44/iZ6Wq",
                FirstName = "Sales",
                LastName = "Manager",
                Role = "Manager",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new User
            {
                Id = 3,
                Email = "user@example.com",
                PasswordHash = "$2a$11$eE6M15vG./4J3y11.w77HekvR/c.26l.7nC4XvK8W2/fT44/iZ6Wq",
                FirstName = "Regular",
                LastName = "User",
                Role = "User",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
