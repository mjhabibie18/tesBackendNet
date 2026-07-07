// ============================================================
// Nama File: Order.cs — Entitas Order dengan Foreign Key & Composite Index
// Folder: 06-DatabaseDesign/Source/Models/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Merepresentasikan tabel pesanan yang terhubung ke User melalui Foreign Key.
//    - Mengapa Diperlukan: Mendemonstrasikan relasi antar tabel, Composite Index, dan nilai default kolom.
//    - Hubungan File: Order.cs bergantung pada User.cs melalui UserId (Foreign Key).
//    - Jika Dihapus: Tabel Orders hilang beserta seluruh relasi ke User.
// ============================================================

namespace TesBackendNet.DatabaseDesign.Models;

/// <summary>
/// TUJUAN CLASS:
/// Entitas pesanan (Order) yang mendemonstrasikan desain tabel anak (child table) dengan FK dan index komposit.
/// 
/// KONSEP COMPOSITE INDEX:
/// Index komposit adalah index yang mencakup lebih dari satu kolom secara bersamaan.
/// Pada entitas ini, contoh composite index: (UserId, OrderDate).
/// Keuntungan: Query seperti "Ambil semua order milik User X dalam 30 hari terakhir" 
/// menjadi sangat cepat karena database dapat menggunakan index ini secara efisien.
/// </summary>
public class Order
{
    /// <summary>
    /// FUNGSI PROPERTY: Primary Key pesanan (auto-increment).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Total nilai transaksi pesanan dalam Rupiah.
    /// ALASAN TIPE DATA (decimal): Presisi finansial tinggi. Menghindari floating-point rounding error.
    /// ALASAN CHECK CONSTRAINT: TotalAmount tidak boleh bernilai negatif.
    /// Dikonfigurasi: `HasCheckConstraint("CK_Orders_TotalAmount", "TotalAmount >= 0")`.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Status pemrosesan pesanan.
    /// DEFAULT VALUE "Pending": Setiap pesanan baru dimulai dengan status "Pending" secara otomatis.
    /// Nilai yang valid: "Pending", "Processing", "Completed", "Cancelled".
    /// CATATAN: Untuk validasi terbatas (enum-like), dapat menggunakan CHECK constraint di database:
    /// `HasCheckConstraint("CK_Orders_Status", "Status IN ('Pending','Processing','Completed','Cancelled')")`.
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// FUNGSI PROPERTY: Tanggal dan waktu pesanan dibuat (UTC).
    /// KAPAN DIGUNAKAN: Digunakan dalam Composite Index bersama UserId untuk query laporan historis.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Foreign Key — merujuk ke baris User yang membuat pesanan ini.
    /// ALASAN FOREIGN KEY:
    /// SQL Server akan menolak penyisipan Order jika UserId tidak merujuk ke baris User yang valid.
    /// Ini menjaga integritas referensial: tidak ada Order "yatim piatu" (tanpa pemilik).
    /// </summary>
    public int UserId { get; set; }
    
    /// <summary>
    /// FUNGSI PROPERTY: Navigation Property ke User pemilik pesanan ini.
    /// Diisi secara otomatis oleh EF Core saat query menggunakan `.Include(o => o.User)`.
    /// </summary>
    public User User { get; set; } = null!;
}
