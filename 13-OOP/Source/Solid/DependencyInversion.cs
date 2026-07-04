// ============================================================
// Nama File: DependencyInversion.cs — Demo SOLID: Dependency Inversion Principle (DIP)
// Folder: 13-OOP/Source/Solid/
// ============================================================
// 1. PENJELASAN FOLDER (OOP/Solid):
//    - Tujuan: Memberikan contoh praktis dan visual tentang penerapan prinsip SOLID dalam pengembangan software.
//    - Kapan Digunakan: Saat merancang struktur class agar mudah dimaintain, ditest, dan diperluas.
//    - Hubungan: Menjadi landasan teoritis bagi modul-modul lain (seperti CRUD dan Authentication) dalam mengimplementasikan loose coupling.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Memperbandingkan penulisan kode yang melanggar DIP (bad practice) dengan yang mematuhi DIP (best practice).
//    - Mengapa Diperlukan: Membantu developer memahami perbedaan nyata antara tight coupling (ketergantungan ketat) dengan loose coupling (ketergantungan longgar).
//    - Hubungan File: Dipanggil dan didemonstrasikan di Program.cs pada modul OOP.
// ============================================================

namespace TesBackendNet.OOP.Solid;

// ── BENTUK SALAH (Melanggar DIP) ─────────────────────────────
// Keterangan Pelanggaran:
// BadProductService (High-Level Module) membuat instance langsung dari SqlProductRepository (Low-Level Module).
// Ini menciptakan "Tight Coupling". Jika di masa mendatang database diganti ke MongoDB, kita harus mengubah isi
// dari class BadProductService. Hal ini melanggar Open/Closed Principle (OCP) dan Dependency Inversion Principle (DIP).

/// <summary>
/// TUJUAN CLASS: Repositori konkrit yang terikat langsung pada database SQL Server.
/// ALASAN TIPE DATA/CLASS: Digunakan langsung oleh BadProductService.
/// </summary>
public class SqlProductRepository
{
    /// <summary>
    /// FUNGSI METHOD: Menyimpan data produk ke SQL Server.
    /// PARAMETER: product (nama produk).
    /// </summary>
    public void Save(string product) => 
        Console.WriteLine($"SqlRepo: Menyimpan {product} ke SQL Server");
}

/// <summary>
/// TUJUAN CLASS: Service untuk memproses produk yang melanggar prinsip DIP.
/// ALASAN KELAS INI BURUK: Bergantung langsung pada detail implementasi (SqlProductRepository) bukan abstraksi.
/// </summary>
public class BadProductService
{
    // Hardcoded dependency (Ketergantungan ketat). Sulit untuk di-unit test karena selalu memanggil SQL Server asli.
    private readonly SqlProductRepository _repo = new();

    /// <summary>
    /// FUNGSI METHOD: Menambahkan produk baru.
    /// ALUR EKSEKUSI: Langsung meneruskan data ke repo konkrit.
    /// </summary>
    public void AddProduct(string name)
    {
        _repo.Save(name);
    }
}

// ── BENTUK BENAR (Memenuhi DIP) ──────────────────────────────
// Penjelasan Teori DIP:
// 1. High-level module tidak boleh bergantung pada low-level module. Keduanya harus bergantung pada abstraksi (Interface).
// 2. Abstraksi tidak boleh bergantung pada detail. Detail (class konkrit) harus bergantung pada abstraksi.

/// <summary>
/// TUJUAN INTERFACE:
/// Bertindak sebagai kontrak (abstraksi) untuk operasi data produk.
/// 
/// MENGAPA MEMAKAI INTERFACE:
/// Memungkinkan sistem menjadi modular. Kita bisa dengan mudah menukar implementasi penyimpanan data
/// (misal SQL ke MongoDB, atau SQL ke InMemoryMock untuk Unit Testing) tanpa mengubah baris kode apa pun di Service Layer.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// FUNGSI METHOD: Kontrak untuk menyimpan data produk.
    /// PARAMETER: product (nama produk yang disimpan).
    /// </summary>
    void Save(string product);
}

/// <summary>
/// TUJUAN CLASS: Implementasi repositori berbasis SQL Server yang mematuhi kontrak.
/// </summary>
public class SqlRepository : IProductRepository
{
    /// <summary>
    /// FUNGSI METHOD: Menyimpan data ke SQL Server.
    /// ALASAN IMPLEMENTASI: Mengimplementasikan method Save dari interface IProductRepository.
    /// </summary>
    public void Save(string product) => 
        Console.WriteLine($"[SQL Repository] Menyimpan '{product}' ke SQL Server Database.");
}

/// <summary>
/// TUJUAN CLASS: Implementasi repositori berbasis MongoDB.
/// </summary>
public class MongoRepository : IProductRepository
{
    /// <summary>
    /// FUNGSI METHOD: Menyimpan data ke MongoDB.
    /// </summary>
    public void Save(string product) => 
        Console.WriteLine($"[Mongo Repository] Menyimpan '{product}' ke Mongo Document Database.");
}

/// <summary>
/// TUJUAN CLASS: Service untuk memproses produk yang mematuhi prinsip DIP.
/// 
/// ALASAN PENGGUNAAN CLASS INI:
/// Memiliki tingkat ketergantungan yang fleksibel karena bergantung sepenuhnya pada IProductRepository (abstraksi).
/// </summary>
public class GoodProductService
{
    // Bergantung pada abstraksi (Interface), bukan class konkrit
    private readonly IProductRepository _repository;

    /// <summary>
    /// CONSTRUCTOR (Dependency Injection):
    /// Menerapkan pola Inversion of Control (IoC) di mana dependensi IProductRepository 
    /// disuntikkan dari luar (misalnya dari Program.cs) ke dalam class ini.
    /// </summary>
    /// <param name="repository">Implementasi repositori apa saja yang memenuhi kontrak IProductRepository.</param>
    public GoodProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// FUNGSI METHOD: Memproses dan menambahkan produk baru.
    /// PARAMETER: name (nama produk).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Mencetak log penerimaan produk.
    /// 2. Memanggil method Save pada interface `_repository`.
    /// 3. Kelas ini tidak peduli apakah data disimpan ke SQL, MongoDB, atau Mock File; ia hanya mempercayai kontrak.
    /// </summary>
    public void AddProduct(string name)
    {
        Console.WriteLine($"[Good Product Service] Memproses produk: {name}");
        _repository.Save(name); // Memanggil implementasi yang di-inject saat runtime
    }
}
