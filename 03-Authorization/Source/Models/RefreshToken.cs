// ============================================================
// RefreshToken.cs — Model Refresh Token
// ============================================================

namespace TesBackendNet.Authorization.Models;

public class RefreshToken
{
    public int Id { get; set; }
    
    public string Token { get; set; } = string.Empty;
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByIp { get; set; }
    
    public bool IsRevoked { get; set; } = false;
    
    public bool IsActive => !IsRevoked && !IsExpired;
}
