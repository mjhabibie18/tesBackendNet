// ============================================================
// UserConfiguration.cs — Konfigurasi Skema Tabel Users
// ============================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TesBackendNet.DatabaseDesign.Models;

namespace TesBackendNet.DatabaseDesign.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        // 1. Primary Key (PK) - Surrogate Key
        builder.HasKey(u => u.Id);

        // 2. Unique Index (Natural Key) - Menjamin email tidak duplikat
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();

        // 3. Default Values (Constraints)
        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()"); // Default value server SQL Server

        // 4. CHECK Constraint (Usia minimal 0 dan maksimal 150)
        builder.ToTable(t => t.HasCheckConstraint("CK_Users_Age", "Age >= 0 AND Age <= 150"));
    }
}
