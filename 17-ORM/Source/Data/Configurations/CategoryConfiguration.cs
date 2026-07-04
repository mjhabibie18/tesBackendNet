// ============================================================
// CategoryConfiguration.cs — IEntityTypeConfiguration
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesBackendNet.ORM.Models;

namespace TesBackendNet.ORM.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Relasi 1-to-Many dikonfigurasi secara eksplisit (Fluent API)
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Cascade); // Jika category dihapus, hapus produk di dalamnya
    }
}
