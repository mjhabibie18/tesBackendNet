// ============================================================
// Category.cs — Model Category (Entitas Utama)
// ============================================================

namespace TesBackendNet.ORM.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation Property: Satu Category punya banyak Products (1-to-N)
    public List<Product> Products { get; set; } = new();
}
