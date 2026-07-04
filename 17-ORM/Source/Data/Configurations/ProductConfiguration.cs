// ============================================================
// ProductConfiguration.cs — IEntityTypeConfiguration
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesBackendNet.ORM.Models;

namespace TesBackendNet.ORM.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Price)
            .HasPrecision(18, 2); // Presisi desimal di database (18 digit, 2 desimal)

        // Indeks untuk mempercepat pencarian pencarian nama
        builder.HasIndex(p => p.Name);

        // Global Query Filter: otomatis menyaring data soft-deleted (IsDeleted = false)
        // di semua query LINQ EF Core secara otomatis!
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
