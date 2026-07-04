// ============================================================
// Product.cs — Model Product (Entitas Utama)
// ============================================================

namespace TesBackendNet.ORM.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; } = false;

    // ── Relasi 1-to-Many ──────────────────────────────────────
    public int CategoryId { get; set; }
    
    // Navigation Property ke parent Category
    public Category Category { get; set; } = null!;
}
