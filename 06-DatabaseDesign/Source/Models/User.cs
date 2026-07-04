// ============================================================
// User.cs — Entitas User dengan Natural Unique Key
// ============================================================

namespace TesBackendNet.DatabaseDesign.Models;

public class User
{
    // Surrogate Primary Key
    public int Id { get; set; }

    // Natural Key (Email & Username) - di-configure Unique di Database
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public int Age { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    // Navigation Property ke Order (1-to-Many)
    public List<Order> Orders { get; set; } = new();
}
