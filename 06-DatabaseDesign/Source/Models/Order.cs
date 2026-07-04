// ============================================================
// Order.cs — Entitas Order dengan Composite Index
// ============================================================

namespace TesBackendNet.DatabaseDesign.Models;

public class Order
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Default value
    public DateTime OrderDate { get; set; }

    // Foreign Key ke tabel Users
    public int UserId { get; set; }
    
    // Navigation Property ke User (Parent)
    public User User { get; set; } = null!;
}
