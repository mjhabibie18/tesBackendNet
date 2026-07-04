// ============================================================
// User.cs — Model User dengan Role
// ============================================================
// Di modul Authorization, Role sangat penting.
// Role menentukan permission apa yang dimiliki user ini.
// ============================================================

namespace TesBackendNet.Authorization.Models;

public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; }

    // ── Role untuk Authorization ──────────────────────────────
    // Menyimpan role user (misal: "Admin", "Manager", "User")
    // Dalam skenario lebih kompleks, bisa dipisah ke tabel UserRoles
    // namun untuk sebagian besar API, string sederhana sudah cukup.
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }
}
