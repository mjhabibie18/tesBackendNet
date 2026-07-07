// ============================================================
// Nama File: User.cs — Entitas User dengan Surrogate PK & Natural Unique Keys
// Folder: 06-DatabaseDesign/Source/Models/
// ============================================================
// 1. PENJELASAN FOLDER (DatabaseDesign/Models):
//    - Tujuan: Mendefinisikan model entitas yang menerapkan konsep desain database relasional modern:
//      Primary Key, Unique Constraint, Check Constraint, Index, Foreign Key, dan Normalisasi.
//    - Kapan Digunakan: Saat merancang skema database baru agar efisien, konsisten, dan bebas anomali data.
//    - Hubungan: Digunakan oleh AppDbContext.cs via DbSet dan dikonfigurasi lebih lanjut via Fluent API.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Merepresentasikan entitas pengguna (User) dengan desain kolom yang baik.
//    - Mengapa Diperlukan: Menunjukkan konsep Surrogate PK vs Natural Key dan cara menerapkan Unique constraint.
//    - Hubungan File: User.cs berelasi ke Order.cs sebagai entitas induk (One-to-Many).
//    - Jika Dihapus: Tabel Users dan seluruh relasi tidak dapat dibuat.
// ============================================================

namespace TesBackendNet.DatabaseDesign.Models;

/// <summary>
/// TUJUAN CLASS:
/// Entitas User yang mendemonstrasikan beberapa konsep desain database penting.
/// 
/// KONSEP: SURROGATE KEY vs NATURAL KEY
/// - Surrogate Key (Id/int): ID buatan sistem yang tidak memiliki makna bisnis.
///   Keunggulan: Stabil (tidak berubah saat data bisnis berubah), ukuran kecil (4 byte int vs string).
/// - Natural Key (Email/Username): Nilai yang sudah ada maknanya di dunia nyata.
///   Masalah: Natural Key sering berubah (pengguna ganti email) dan lebih besar ukurannya sebagai FK.
/// 
/// BEST PRACTICE: Gunakan Surrogate Key (int/Guid) sebagai Primary Key, 
/// dan Natural Key sebagai Unique Constraint terpisah.
/// </summary>
public class User
{
    /// <summary>
    /// FUNGSI PROPERTY: Surrogate Primary Key — ID numerik buatan sistem.
    /// ALASAN (int vs Guid): int lebih kecil (4 byte) dan query lebih cepat. 
    /// Gunakan Guid jika memerlukan ID yang dapat digenerate di sisi klien tanpa database (distributed systems).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Nama pengguna yang unik di seluruh sistem.
    /// ALASAN UNIQUE CONSTRAINT: Mencegah dua user terdaftar dengan username yang sama.
    /// Dikonfigurasi via `HasIndex(u => u.Username).IsUnique()` di Fluent API / Entity Configuration.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Alamat email yang unik dan berfungsi sebagai identitas login.
    /// ALASAN UNIQUE + INDEX: Email sering digunakan untuk WHERE clause (misal: login by email),
    /// sehingga penambahan Index mempercepat pencarian O(n) → O(log n).
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Usia pengguna dalam tahun.
    /// ALASAN CHECK CONSTRAINT: Nilai usia tidak boleh negatif atau tidak masuk akal (misal: > 150).
    /// Dikonfigurasi di Entity Configuration: `HasCheckConstraint("CK_Users_Age", "Age >= 0 AND Age <= 150")`.
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Status aktif pengguna (soft enable/disable).
    /// DEFAULT TRUE: Pengguna baru diasumsikan aktif saat pertama kali terdaftar.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// FUNGSI PROPERTY: Waktu pendaftaran pengguna (UTC).
    /// ALASAN UTC: Menyimpan dalam UTC (Coordinated Universal Time) memastikan tidak ada ambiguitas zona waktu.
    /// Konversi ke zona waktu lokal pengguna dilakukan di lapisan presentasi (frontend), bukan database.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Navigation Property ke daftar pesanan milik user ini.
    /// Mewakili sisi "Satu" dalam relasi One-to-Many: 1 User → Banyak Order.
    /// </summary>
    public List<Order> Orders { get; set; } = new();
}
