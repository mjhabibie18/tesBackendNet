// ============================================================
// RefreshToken.cs — Model untuk Refresh Token
// ============================================================
// Refresh Token disimpan di database agar bisa:
//   1. Di-revoke kapan saja (logout, security breach)
//   2. Dilacak (audit: device mana yang login)
//   3. Dibatasi per device
// ============================================================

namespace TesBackendNet.Authentication.Models;

/// <summary>
/// Merepresentasikan Refresh Token yang disimpan di database.
/// Setiap login menghasilkan satu Refresh Token baru.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }

    /// <summary>
    /// Token string yang random dan unik.
    /// Di-generate menggunakan cryptographically secure random bytes.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Kapan token ini expire.
    /// Biasanya 7-30 hari setelah dibuat.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Apakah token sudah digunakan?
    /// Setelah dipakai refresh, token lama di-revoke.
    /// </summary>
    public bool IsRevoked { get; set; } = false;

    /// <summary>Kapan token dibuat</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>IP address user saat login (untuk audit)</summary>
    public string? CreatedByIp { get; set; }

    // ── Foreign Key ───────────────────────────────────────────
    /// <summary>ID user yang punya refresh token ini</summary>
    public int UserId { get; set; }

    // Navigation Property — relasi ke User
    public User User { get; set; } = null!;

    // ── Computed Property ─────────────────────────────────────
    /// <summary>Apakah token masih aktif (belum expire dan belum di-revoke)?</summary>
    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;
}
