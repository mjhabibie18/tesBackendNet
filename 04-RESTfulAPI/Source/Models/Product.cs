// ============================================================
// Nama File: Product.cs — Model Database Entitas Produk
// Folder: 04-RESTfulAPI/Source/Models/
// ============================================================
// 1. PENJELASAN FOLDER (RESTfulAPI/Source/Models):
//    - Tujuan: Menyimpan definisi struktur data (Entity/Model) yang dipetakan langsung ke tabel database.
//    - Kapan Digunakan: Saat merancang skema database relasional di tingkat kode (Code-First approach).
//    - Hubungan: Digunakan oleh DbContext untuk membentuk skema tabel, dan oleh Repository untuk manipulasi baris data.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Merepresentasikan struktur satu baris data di dalam tabel 'Products'.
//    - Mengapa Diperlukan: Entity Framework Core membutuhkan class model konkrit sebagai acuan pembuatan kolom database dan tipe datanya.
//    - Hubungan File: Dipetakan ke DbSet di AppDbContext.cs dan digunakan sebagai data transfer di Controller.
//    - Jika Dihapus: Aplikasi tidak akan memiliki definisi entitas Product, menyebabkan kueri database pada produk tidak dapat dibuat.
// ============================================================

namespace TesBackendNet.RESTfulAPI.Models;

/// <summary>
/// TUJUAN CLASS:
/// Kelas domain model utama yang mewakili entitas produk dalam database.
/// 
/// PRINSIP OOP:
/// - Abstraksi Data: Memodelkan objek dunia nyata (Produk) ke dalam property-property terstruktur C#.
/// - Anemic Domain Model: Class ini murni menyimpan state/data tanpa mengandung logika bisnis yang kompleks.
/// </summary>
public class Product
{
    /// <summary>
    /// FUNGSI PROPERTY: Bertindak sebagai Primary Key (PK) untuk membedakan unik setiap baris produk.
    /// ALASAN TIPE DATA (int): Bilangan bulat yang secara otomatis bertambah (Identity 1,1) di database SQL Server.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan nama produk.
    /// ALASAN TIPE DATA (string): Terdiri atas kumpulan karakter alfanumerik.
    /// KAPAN DIGUNAKAN: Ditampilkan di katalog produk dan digunakan saat pencarian (search query).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan deskripsi atau penjelasan lengkap mengenai detail produk.
    /// ALASAN TIPE DATA (string?): Nullable string (menggunakan tanda ?) karena deskripsi bersifat opsional (boleh null/kosong di database).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Menunjukkan harga jual produk.
    /// ALASAN TIPE DATA (decimal): Tipe data presisi 128-bit yang sangat krusial untuk finansial demi menghindari bug pembulatan float/double.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Jumlah stok barang yang tersedia di gudang.
    /// ALASAN TIPE DATA (int): Jumlah barang bernilai bulat (tidak ada pecahan desimal untuk jumlah fisik produk).
    /// </summary>
    public int Stock { get; set; }

    /// <summary>
    /// FUNGSI PROPERTY: Waktu pencatatan produk saat dimasukkan ke sistem.
    /// ALASAN TIPE DATA (DateTime): Menyimpan informasi tanggal dan waktu presisi tinggi.
    /// KAPAN DIGUNAKAN: Default terisi waktu saat ini dalam format Coordinated Universal Time (UTC) untuk konsistensi zona waktu.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
