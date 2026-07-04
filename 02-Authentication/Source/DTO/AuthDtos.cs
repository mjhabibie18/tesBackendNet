// ============================================================
// AuthDtos.cs — DTO untuk Authentication
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace TesBackendNet.Authentication.DTO;

// ── REGISTER ─────────────────────────────────────────────────
/// <summary>Request DTO untuk registrasi user baru.</summary>
public class RegisterDto
{
    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    [StringLength(200, ErrorMessage = "Email maksimal 200 karakter")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi")]
    [StringLength(100, MinimumLength = 8,
        ErrorMessage = "Password minimal 8 karakter, maksimal 100 karakter")]
    // Regex untuk validasi password kuat:
    // Minimal 1 huruf besar, 1 huruf kecil, 1 angka
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password harus mengandung huruf besar, huruf kecil, dan angka")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konfirmasi password wajib diisi")]
    [Compare(nameof(Password), ErrorMessage = "Konfirmasi password tidak sama dengan password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nama depan wajib diisi")]
    [StringLength(100, ErrorMessage = "Nama depan maksimal 100 karakter")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Nama belakang maksimal 100 karakter")]
    public string? LastName { get; set; }
}

// ── LOGIN ─────────────────────────────────────────────────────
/// <summary>Request DTO untuk login.</summary>
public class LoginDto
{
    [Required(ErrorMessage = "Email wajib diisi")]
    [EmailAddress(ErrorMessage = "Format email tidak valid")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password wajib diisi")]
    public string Password { get; set; } = string.Empty;
}

// ── REFRESH TOKEN ─────────────────────────────────────────────
/// <summary>Request DTO untuk refresh access token.</summary>
public class RefreshTokenDto
{
    [Required(ErrorMessage = "Refresh token wajib diisi")]
    public string RefreshToken { get; set; } = string.Empty;
}

// ── TOKEN RESPONSE ────────────────────────────────────────────
/// <summary>
/// Response setelah login berhasil.
/// Berisi Access Token dan Refresh Token.
/// </summary>
public class AuthTokenDto
{
    /// <summary>JWT Access Token — kirim di Authorization header setiap request</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Refresh Token — simpan aman, gunakan hanya untuk refresh</summary>
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>Kapan Access Token expire</summary>
    public DateTime AccessTokenExpiry { get; set; }

    /// <summary>Kapan Refresh Token expire</summary>
    public DateTime RefreshTokenExpiry { get; set; }

    /// <summary>Tipe token: selalu "Bearer"</summary>
    public string TokenType { get; set; } = "Bearer";
}

// ── USER RESPONSE ─────────────────────────────────────────────
/// <summary>Response data user (untuk profile atau setelah register).</summary>
public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

// ── LOGIN RESPONSE ────────────────────────────────────────────
/// <summary>Response lengkap setelah login: user info + tokens.</summary>
public class LoginResponseDto
{
    public UserResponseDto User { get; set; } = null!;
    public AuthTokenDto Tokens { get; set; } = null!;
}
