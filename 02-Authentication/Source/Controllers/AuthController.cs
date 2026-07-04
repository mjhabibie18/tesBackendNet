// ============================================================
// Nama File: AuthController.cs — Controller Autentikasi Utama
// Folder: 02-Authentication/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Authentication):
//    - Tujuan: Mengatur registrasi pengguna, otentikasi login, serta manajemen token (JWT & Refresh Token).
//    - Kapan Digunakan: Ketika pengguna ingin masuk ke sistem atau memperbarui token akses mereka.
//    - Hubungan: Terikat langsung dengan AuthService dan TokenService untuk menghasilkan kredensial yang valid.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan REST endpoints guna interaksi otentikasi pengguna dari client.
//    - Mengapa Diperlukan: Sebagai satu-satunya pintu masuk bagi pengguna luar untuk memperoleh token otentikasi.
//    - Hubungan File: Memanggil IAuthService untuk mengeksekusi logika validasi kredensial pengguna.
//    - Jika Dihapus: Pengguna tidak dapat mendaftar, login, logout, atau melakukan refresh token pada sistem.
// ============================================================

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesBackendNet.Authentication.DTO;
using TesBackendNet.Authentication.Services;

namespace TesBackendNet.Authentication.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Mengekspos REST endpoints untuk pendaftaran akun, verifikasi login, pergantian token, dan pengambilan profil pengguna.
/// 
/// ALASAN PENGGUNAAN CONTROLLER:
/// Menerapkan pola MVC (Model-View-Controller) / Web API untuk memisahkan urusan protokol HTTP (request/response) 
/// dari core business logic yang berada di tingkat Service.
/// 
/// LIFECYCLE:
/// Scoped / Transient. Instansiasi baru dari controller ini dibuat oleh ASP.NET Core framework 
/// untuk setiap HTTP request yang masuk, lalu dihancurkan (garbage collected) setelah response terkirim.
/// 
/// DEPENDENCY:
/// - IAuthService: Berisi logika bisnis otentikasi (registrasi, validasi password, rotasi token).
/// - ILogger: Service logging untuk memantau aktivitas login dan mencatat anomali keamanan.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// CONSTRUCTOR: Menerapkan Dependency Injection (DI) untuk menyuntikkan AuthService dan Logger.
    /// </summary>
    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger      = logger;
    }

    /// <summary>
    /// FUNGSI METHOD: Mendaftarkan pengguna baru ke sistem.
    /// PARAMETER: RegisterDto (berisi Username, Email, dan Password mentah).
    /// NILAI KEMBALIAN: Task<IActionResult> (HTTP status code 201 Created bersama payload data pengguna baru).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Controller menerima HTTP POST request dengan JSON body.
    /// 2. Data di-deserialize menjadi RegisterDto.
    /// 3. Memanggil `await _authService.RegisterAsync(dto)`.
    /// 4. Mengembalikan HTTP 201 Created menggunakan CreatedAtAction.
    /// 
    /// BEST PRACTICE:
    /// Gunakan status code 201 Created (melalui CreatedAtAction/Created) saat berhasil membuat entitas baru di sistem.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        
        // CreatedAtAction mengembalikan header Location yang mengarah ke endpoint peninjauan profil.
        return CreatedAtAction(nameof(GetProfile), null,
            new { success = true, message = "Registrasi berhasil", data = result });
    }

    /// <summary>
    /// FUNGSI METHOD: Memverifikasi kredensial pengguna untuk login.
    /// PARAMETER: LoginDto (Email dan Password).
    /// NILAI KEMBALIAN: Task<IActionResult> (HTTP 200 OK bersama JWT Access Token dan Refresh Token).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Controller menerima request login.
    /// 2. Mengambil alamat IP pengirim untuk keperluan audit keamanan.
    /// 3. Memanggil `_authService.LoginAsync`.
    /// 4. Jika sukses, menghasilkan JWT dan Refresh Token lalu mengembalikan payload sukses.
    /// 
    /// BARIS KODE PENTING:
    /// `Request.HttpContext.Connection.RemoteIpAddress` digunakan untuk mencatat IP klien. 
    /// Ini sangat penting untuk mendeteksi upaya login brute force atau anomali geolokasi.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.LoginAsync(dto, ipAddress);
        return Ok(new { success = true, message = "Login berhasil", data = result });
    }

    /// <summary>
    /// FUNGSI METHOD: Memperbarui JWT Access Token yang telah kadaluarsa menggunakan Refresh Token yang valid.
    /// PARAMETER: RefreshTokenDto (String token penyegar).
    /// NILAI KEMBALIAN: Task<IActionResult> (Access Token baru beserta Refresh Token baru - Token Rotation).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Menerima request berisi refresh token lama.
    /// 2. Memanggil `_authService.RefreshTokenAsync`.
    /// 3. Menghapus refresh token lama di DB dan mengeluarkan pasangan token baru (rotasi).
    /// 
    /// BEST PRACTICE:
    /// Terapkan Refresh Token Rotation (mengeluarkan refresh token baru setiap kali melakukan refresh) 
    /// untuk mencegah replay attack jika refresh token lama dicuri.
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ipAddress);
        return Ok(new { success = true, message = "Token berhasil diperbarui", data = result });
    }

    /// <summary>
    /// FUNGSI METHOD: Menghapus sesi otentikasi (logout) pengguna.
    /// PARAMETER: RefreshTokenDto.
    /// NILAI KEMBALIAN: Task<IActionResult> (HTTP 200 OK).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Memanggil `_authService.LogoutAsync`.
    /// 2. Mencari refresh token di database lalu menghapusnya secara permanen (revoke).
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        await _authService.LogoutAsync(dto.RefreshToken);
        return Ok(new { success = true, message = "Logout berhasil" });
    }

    /// <summary>
    /// FUNGSI METHOD: Mengambil data profil milik pengguna yang sedang login.
    /// PARAMETER: Tidak ada parameter eksplisit (diambil dari Claims JWT).
    /// NILAI KEMBALIAN: Task<IActionResult> (HTTP 200 OK berisi profil detail).
    /// 
    /// BARIS KODE PENTING:
    /// - [Authorize]: Atribut penanda bahwa endpoint ini dilindungi. Hanya request dengan token JWT valid yang diperbolehkan masuk.
    /// - `User.FindFirst(ClaimTypes.NameIdentifier)`: Mengambil klaim "sub" (User ID) dari payload JWT terenkripsi yang didekode otomatis oleh ASP.NET Core Authentication middleware.
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        // ── 1. Mengambil User ID dari Claims Principal ───────────────────────────
        // Saat JWT divalidasi oleh framework, isi token (claims) didekode dan dimasukkan
        // ke dalam properti 'User' (ClaimsPrincipal) dari controller.
        // - ClaimTypes.NameIdentifier: Klaim standar XML namespace yang biasanya memetakan ke JWT claim "sub".
        // - "userId": Custom claim cadangan jika pengembang menggunakan nama klaim non-standar.
        // Gunakan operator Null Coalescing (??) sebagai fallback pencarian.
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("userId")?.Value;

        // ── 2. Validasi & Parsing Tipe Data User ID ──────────────────────────────
        // Nilai klaim dari JWT selalu berbentuk string. Karena Primary Key (PK) User 
        // di database kita adalah bertipe 'int', kita wajib mengubahnya (parsing).
        // - int.TryParse: Mencegah error runtime (FormatException) jika nilai string tidak valid/null.
        // - out var userId: Mendeklarasikan variabel penampung integer jika parsing berhasil.
        // - Jika gagal: Kembalikan HTTP 401 Unauthorized karena token dianggap rusak/tidak valid.
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { success = false, message = "Token tidak valid" });

        // ── 3. Pengambilan Profil Melalui Service Layer (Asinkron) ────────────────
        // Memanggil service untuk query ke database.
        // - await: Menghentikan eksekusi method ini secara asinkron (non-blocking) sementara 
        //   database bekerja, membebaskan thread web server untuk melayani request lain.
        var result = await _authService.GetProfileAsync(userId);
        
        // ── 4. Antisipasi Skenario Kasus Khusus (Edge Case) ──────────────────────
        // Apa yang terjadi jika User ID ada di token, tetapi user telah dihapus di DB?
        // (Misalnya admin menghapus akun user, tetapi token JWT user tersebut belum kedaluwarsa).
        // - Jika result bernilai null: Kembalikan HTTP 404 Not Found.
        if (result == null)
            return NotFound(new { success = false, message = "User tidak ditemukan" });

        // ── 5. Pengembalian Hasil Sukses ──────────────────────────────────────────
        // Mengembalikan HTTP 200 OK beserta payload profil pengguna yang terbungkus
        // dalam standard response envelope.
        return Ok(new { success = true, data = result });
    }
}
