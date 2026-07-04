// ============================================================
// Product.cs — Model / Entity
// ============================================================
// Model (atau Entity) adalah representasi dari tabel di database.
// Setiap property di class ini = satu kolom di tabel Products.
//
// Mengapa membuat Model terpisah?
//   - Single Responsibility: satu class untuk satu tujuan
//   - Entity Framework menggunakan class ini untuk membuat/query tabel
//   - Bisa menambahkan DataAnnotations untuk validasi & constraint
// ============================================================

namespace TesBackendNet.CRUD.Models;

/// <summary>
/// Merepresentasikan entitas Product di database.
/// Setiap instance = satu baris di tabel Products.
/// </summary>
public class Product
{
    // ── Primary Key ──────────────────────────────────────────
    // Konvensi EF Core: property bernama "Id" atau "{ClassName}Id"
    // otomatis dikenali sebagai Primary Key.
    // EF Core juga set IDENTITY(1,1) di SQL Server secara otomatis.
    public int Id { get; set; }

    // ── Required Fields ──────────────────────────────────────
    // string.Empty = default value untuk mencegah null warning
    // Di SQL Server: kolom ini akan menjadi NOT NULL VARCHAR
    public string Name { get; set; } = string.Empty;

    // ── Nullable Fields ──────────────────────────────────────
    // string? = nullable reference type (bisa null)
    // Di SQL Server: kolom ini akan menjadi NULL VARCHAR
    public string? Description { get; set; }

    // ── Decimal Fields ───────────────────────────────────────
    // decimal: untuk nilai uang/harga, JANGAN pakai float/double
    // float/double: tidak presisi untuk nilai desimal (rounding error)
    // decimal: presisi exact, aman untuk finansial
    //
    // Di SQL Server: default decimal(18,2) — bisa dikonfigurasi di DbContext
    public decimal Price { get; set; }

    // ── Integer Fields ───────────────────────────────────────
    public int Stock { get; set; }

    // ── Soft Delete Fields ───────────────────────────────────
    // IsDeleted: flag untuk soft delete
    // default false = belum dihapus saat pertama kali dibuat
    //
    // Mengapa Soft Delete?
    //   - Data tidak hilang permanen
    //   - Bisa di-restore jika ada kesalahan
    //   - Audit trail tetap ada
    //   - Relasi ke tabel lain tidak rusak
    public bool IsDeleted { get; set; } = false;

    // ── Audit Fields ─────────────────────────────────────────
    // Audit fields: mencatat kapan dan siapa yang membuat/mengubah data
    // Best practice untuk production: selalu ada audit trail

    /// <summary>Waktu data dibuat. Menggunakan UTC untuk konsistensi timezone.</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Waktu data terakhir diupdate. Null jika belum pernah diupdate.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>Waktu data di-soft delete. Null jika belum dihapus.</summary>
    public DateTime? DeletedAt { get; set; }
}
