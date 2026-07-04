// ============================================================
// User.cs — Model untuk User
// ============================================================
namespace TesBackendNet.Authentication.Models;

/// <summary>
/// Entitas User untuk autentikasi.
/// Merepresentasikan tabel Users di database.
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Email digunakan sebagai username (unique)</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// PENTING: Ini adalah HASH password, bukan plain text!
    /// bcrypt hash format: $2a$11$[22char salt][31char hash]
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    /// <summary>Role user: "Admin", "User", dll</summary>
    public string Role { get; set; } = "User";

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation Property: satu user bisa punya banyak refresh token
    // (login dari banyak device)
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
