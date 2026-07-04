// ============================================================
// Nama File: AppDbContext.cs — Database Context untuk Authentication
// Folder: 02-Authentication/Source/Data/
// ============================================================
// 1. PENJELASAN FOLDER (Authentication):
//    - Tujuan: Mengelola data kredensial, role, dan token sesi pengguna.
//    - Kapan Digunakan: Saat melakukan registrasi, verifikasi login, rotasi token, dan pengecekan akses (autorisasi).
//    - Hubungan: Terkoneksi dengan model User dan RefreshToken untuk memetakan skema otentikasi ke database.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyusun konfigurasi pemetaan (mapping) tabel User dan tabel RefreshToken secara relasional.
//    - Mengapa Diperlukan: Agar EF Core memahami batasan integritas data autentikasi seperti indeks unik untuk email dan aksi cascade delete token.
//    - Hubungan File: Menyediakan akses ke entitas User.cs dan RefreshToken.cs yang dipanggil oleh AuthService.cs.
//    - Jika Dihapus: Aplikasi kehilangan repositori penyimpanan user dan refresh token, sehingga sistem otentikasi tidak dapat berjalan.
// ============================================================

using Microsoft.EntityFrameworkCore;
using TesBackendNet.Authentication.Models;

namespace TesBackendNet.Authentication.Data;

/// <summary>
/// TUJUAN CLASS:
/// Kelas penampung sesi koneksi database yang mengelola tabel pengguna dan token penyegar (Refresh Token).
/// 
/// ALASAN MENGGUNAKAN DbContext:
/// Memungkinkan pemrosesan query LINQ ke SQL Server serta menerapkan constraint unik dan relasi cascade secara terpusat di memori.
/// 
/// LIFECYCLE:
/// Scoped. Dibuat per HTTP request untuk memastikan koneksi database dibuka dan ditutup secara disiplin.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// CONSTRUCTOR: Meneruskan konfigurasi koneksi string ke kelas induk DbContext.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// FUNGSI PROPERTY: Memetakan entity User ke tabel 'Users' di database.
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// FUNGSI PROPERTY: Memetakan entity RefreshToken ke tabel 'RefreshTokens' di database.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    /// <summary>
    /// FUNGSI METHOD: Mengonfigurasi properti, indeks unik, dan hubungan (relationship) antar tabel database.
    /// PARAMETER: ModelBuilder (fluent API builder dari EF Core).
    /// 
    /// ALUR KONFIGURASI PENTING:
    /// 1. Konfigurasi User:
    ///    - `ToTable("Users")`: Menetapkan nama tabel fisik di SQL Server.
    ///    - `HasIndex(u => u.Email).IsUnique()`: Mencegah email ganda terdaftar (indeks unik tingkat database).
    ///    - `HasMany(u => u.RefreshTokens).WithOne(rt => rt.User)`: Mendefinisikan relasi One-to-Many antara User dengan RefreshToken.
    ///    - `OnDelete(DeleteBehavior.Cascade)`: Aturan Cascade Delete. Jika baris User dihapus, seluruh baris token penyegar (refresh token) miliknya di database akan ikut terhapus secara otomatis oleh sistem database demi integritas referensial.
    /// 2. Konfigurasi RefreshToken:
    ///    - `ToTable("RefreshTokens")`: Nama tabel penyimpanan token.
    ///    - `HasIndex(rt => rt.Token).IsUnique()`: Memastikan kerahasiaan token terlindungi dengan indeks unik.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Konfigurasi User ──────────────────────────────────
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(200)
                  .HasColumnType("nvarchar(200)");

            entity.Property(u => u.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(500); // BCrypt hash panjangnya sekitar 60 karakter

            entity.Property(u => u.FirstName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.LastName)
                  .HasMaxLength(100);

            entity.Property(u => u.Role)
                  .IsRequired()
                  .HasMaxLength(50)
                  .HasDefaultValue("User");

            // Email harus unik di database
            entity.HasIndex(u => u.Email)
                  .IsUnique()
                  .HasDatabaseName("IX_Users_Email_Unique");

            // Relasi: satu User punya banyak RefreshToken
            entity.HasMany(u => u.RefreshTokens)
                  .WithOne(rt => rt.User)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // Hapus user = hapus semua refresh tokennya
        });

        // ── Konfigurasi RefreshToken ──────────────────────────
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Token)
                  .IsRequired()
                  .HasMaxLength(500);

            // Index untuk query cepat saat validasi refresh token
            entity.HasIndex(rt => rt.Token)
                  .IsUnique()
                  .HasDatabaseName("IX_RefreshTokens_Token_Unique");
        });
    }
}
