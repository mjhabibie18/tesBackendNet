// ============================================================
// Nama File: Blog.cs — Model Entitas Parent (Blog)
// Folder: 05-Database/Source/Models/
// ============================================================
// 1. PENJELASAN FOLDER (Database/Models):
//    - Tujuan: Mendefinisikan kelas-kelas C# yang dipetakan menjadi tabel di database SQL Server.
//    - Kapan Digunakan: Saat menggunakan EF Core Code-First, semua definisi tabel berasal dari model class.
//    - Hubungan: Digunakan oleh AppDbContext.cs dan DbDemoController.cs.
//
// 2. PENJELASAN FILE (Blog.cs):
//    - Fungsi & Tanggung Jawab: Merepresentasikan tabel 'Blogs' di database. Berfungsi sebagai entitas induk (Parent)
//      dalam relasi One-to-Many dengan entitas Post.
//    - Mengapa Diperlukan: Untuk mendemonstrasikan relasi antar tabel (JOIN) dan teknik pengambilan data relasional menggunakan EF Core Include().
//    - Hubungan File: Blog.cs berelasi ke Post.cs via Navigation Property.
//    - Jika Dihapus: Relasi Blog-Post rusak dan semua query yang melibatkan tabel Blogs tidak dapat dijalankan.
// ============================================================

namespace TesBackendNet.Database.Models;

/// <summary>
/// TUJUAN CLASS:
/// Kelas entitas yang merepresentasikan sebuah Blog (sebagai induk/parent dari kumpulan posting artikel).
/// 
/// ALASAN DESAIN RELASI (One-to-Many):
/// Satu Blog dapat memiliki banyak Post (artikel). Relasi ini adalah pola paling umum di desain basis data relasional.
/// Dalam C#, relasi ini dideklarasikan menggunakan Navigation Property (`List<Post> Posts`).
/// 
/// PRINSIP OOP:
/// - Komposisi: Blog "memiliki" banyak Post, bukan mewarisinya.
/// </summary>
public class Blog
{
    /// <summary>
    /// FUNGSI PROPERTY: Primary Key unik untuk setiap Blog di database.
    /// ALASAN TIPE DATA (int): EF Core mendeteksi property bernama 'Id' atau '[ClassName]Id' dan otomatis menjadikannya
    /// sebagai Primary Key dengan nilai IDENTITY (auto-increment) di SQL Server.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Judul utama blog.
    /// ALASAN TIPE DATA (string): Teks alfanumerik. Dibatasi panjangnya di AppDbContext via `HasMaxLength(150)`.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: URL/domain website blog.
    /// ALASAN TIPE DATA (string): URL terdiri dari karakter-karakter teks khusus (seperti ://, .com, ?=).
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Kumpulan artikel (Post) yang dimiliki oleh Blog ini.
    /// 
    /// ALASAN PENGGUNAAN NAVIGATION PROPERTY:
    /// Navigation Property adalah cara EF Core mendeklarasikan relasi antar entitas tanpa menulis SQL JOIN secara manual.
    /// Saat query menggunakan `.Include(b => b.Posts)`, EF Core secara otomatis menghasilkan SQL JOIN untuk mengambil
    /// semua post yang berelasi dengan blog ini.
    /// 
    /// ALASAN INISIALISASI `new()`:
    /// Inisialisasi collection dengan `new()` mencegah NullReferenceException jika property Posts diakses sebelum 
    /// entity Blog diambil dari database (lazy loading tidak diaktifkan).
    /// </summary>
    public List<Post> Posts { get; set; } = new();
}
