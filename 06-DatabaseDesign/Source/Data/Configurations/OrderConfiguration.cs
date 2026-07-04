// ============================================================
// OrderConfiguration.cs — Konfigurasi Skema Tabel Orders
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesBackendNet.DatabaseDesign.Models;

namespace TesBackendNet.DatabaseDesign.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        // 1. Primary Key (PK)
        builder.HasKey(o => o.Id);

        // 2. Composite Index (Mempercepat query pencarian order berdasarkan Status dan Tanggal Order)
        builder.HasIndex(o => new { o.Status, o.OrderDate })
            .HasDatabaseName("IX_Orders_Status_OrderDate");

        // 3. Default Value
        builder.Property(o => o.Status)
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2);

        // 4. CHECK Constraint (TotalAmount tidak boleh negatif)
        builder.ToTable(t => t.HasCheckConstraint("CK_Orders_TotalAmount", "TotalAmount >= 0"));

        // 5. Foreign Key Relasi (FK) dengan Cascade Delete
        builder.HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Jika user dihapus, pesanan terhapus
    }
}
