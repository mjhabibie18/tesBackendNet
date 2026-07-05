// ============================================================
// Nama File: Post.cs — Model Entitas Child (Post/Artikel)
// Folder: 05-Database/Source/Models/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Merepresentasikan tabel 'Posts' di database. Berfungsi sebagai entitas anak (Child)
//      dalam relasi One-to-Many dengan entitas Blog.
//    - Mengapa Diperlukan: Untuk mendemonstrasikan penggunaan Foreign Key, relasi database, soft delete, dan query LINQ lanjutan.
//    - Hubungan File: Post.cs bergantung pada Blog.cs melalui properti BlogId (FK) dan Blog (Navigation Property).
//    - Jika Dihapus: Relasi Blog-Post rusak, dan seluruh fitur query, filter, serta statistik post tidak dapat berjalan.
// ============================================================

namespace TesBackendNet.Database.Models;

/// <summary>
/// TUJUAN CLASS:
/// Kelas entitas yang merepresentasikan sebuah artikel/posting yang diterbitkan di bawah satu Blog tertentu.
/// 
/// POLA DESAIN SOFT DELETE:
/// Property `IsDeleted` ditambahkan sebagai penanda penghapusan lunak. Data Post yang "dihapus" tetap ada di database
/// namun ditandai dengan IsDeleted = true. Ini mencegah kehilangan data historis yang mungkin masih dibutuhkan di masa mendatang.
/// </summary>
public class Post
{
    /// <summary>
    /// FUNGSI PROPERTY: Primary Key unik untuk setiap Post di database (auto-increment).
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Judul artikel.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Isi konten artikel.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Jumlah total kunjungan/views pada artikel ini.
    /// ALASAN TIPE DATA (int): Jumlah tampilan adalah bilangan bulat positif.
    /// KAPAN DIGUNAKAN: Digunakan dalam kueri agregat seperti SUM(Views), MAX(Views), AVG(Views), dan pengurutan.
    /// </summary>
    public int Views { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Bendera soft delete.
    /// ALASAN TIPE DATA (bool): Hanya dua keadaan yang mungkin: sudah dihapus (true) atau belum dihapus (false).
    /// KAPAN DIGUNAKAN: Digunakan sebagai filter pada setiap kueri agar data yang "dihapus" tidak ditampilkan ke pengguna.
    /// DEFAULT VALUE: false — setiap post baru belum ditandai sebagai terhapus.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// FUNGSI PROPERTY: Foreign Key yang menghubungkan artikel ini ke Blog pemiliknya.
    /// 
    /// MENGAPA INI PENTING (Foreign Key):
    /// Foreign Key adalah nilai ID dari baris di tabel induk (Blog) yang menjadi referensi integritas relasional.
    /// SQL Server akan menolak penyisipan Post jika BlogId tidak merujuk ke Blog yang ada (Referential Integrity).
    /// </summary>
    public int BlogId { get; set; }
    
    /// <summary>
    /// FUNGSI PROPERTY: Navigation Property yang menyambungkan objek Post ke objek Blog induknya.
    /// 
    /// ALASAN MENGGUNAKAN `null!`:
    /// Operator null-forgiving (`!`) digunakan untuk memberi tahu compiler bahwa kita menjamin properti ini tidak akan
    /// null saat digunakan (karena EF Core akan mengisinya saat query dijalankan dengan `.Include()`).
    /// Ini mencegah compiler mengeluarkan warning nullable reference type.
    /// </summary>
    public Blog Blog { get; set; } = null!;
}
