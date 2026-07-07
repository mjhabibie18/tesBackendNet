// ============================================================
// Nama File: Program.cs — Demo Debugging & Solusi Bug Umum di C# / .NET
// Folder: 30-Debugging/Source/
// ============================================================
// 1. PENJELASAN FOLDER (Debugging):
//    - Tujuan: Mendokumentasikan bug paling sering terjadi di aplikasi C#/.NET dan cara pencegahan/solusinya.
//    - Kapan Digunakan: Sebagai referensi saat debugging aplikasi yang crash atau berperilaku tidak terduga.
//    - Hubungan: Pola aman yang didemonstrasikan di sini digunakan di seluruh modul lain (Controller, Service, Repository).
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menampilkan 3 skenario bug klasik beserta solusinya: NullReferenceException,
//      Resource/Connection Leak, dan Async Deadlock.
//    - Mengapa Diperlukan: Bug-bug ini adalah yang paling sering menyebabkan crash di environment produksi.
//      Mengenali dan menghindarinya adalah kompetensi wajib backend developer senior.
//    - Jika Dihapus: Tidak ada demonstrasi debugging fungsional pada modul ini.
// ============================================================

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🔍 DEMO DEBUGGING & SOLUSI BUGS UMUM DI C# / .NET");
Console.WriteLine("============================================================\n");

// ── BUG #1: NullReferenceException ───────────────────────────────────
// KAPAN TERJADI: Saat mengakses properti atau method dari referensi objek yang bernilai null.
// PESAN ERROR: "Object reference not set to an instance of an object."
// PENYEBAB UMUM: Service mengembalikan null (misal: GetById tidak menemukan data), tapi caller
//               langsung mengakses properti hasilnya tanpa memeriksa null terlebih dahulu.
Console.WriteLine("--- 1. NULLREFERENCEEXCEPTION PREVENTION ---");
Product? dummyProduct = null; // Simulasi: query ke database tidak menemukan data (return null)

try
{
    // ❌ SALAH: Mengakses .Category.Name langsung pada objek null → crash!
    // ! adalah null-forgiving operator, menginstruksikan compiler untuk abaikan warning.
    // Tanpa penanganan null yang tepat, baris ini akan selalu melempar NullReferenceException.
    Console.WriteLine($"[Dirty] Nama Kategori: {dummyProduct!.Category.Name}");
}
catch (NullReferenceException)
{
    Console.WriteLine("[Dirty] Terjadi NullReferenceException! Program crash jika tidak ditangkap.");
}

// ✅ BENAR: Gunakan Null-Conditional (?.) dan Null-Coalescing (??)
// - `?.` : Jika objek di kiri null, ekspresi langsung mengembalikan null (tidak crash).
// - `??` : Jika nilai di kiri null, gunakan nilai default di kanan.
// Dibaca: "Jika dummyProduct tidak null, akses .Category; jika tidak null, akses .Name; jika hasilnya null, gunakan default."
string categoryName = dummyProduct?.Category?.Name ?? "Kategori Tidak Tersedia";
Console.WriteLine($"[Clean] Nama Kategori: {categoryName}");
Console.WriteLine();

// ── BUG #2: Resource/Connection Leak (Kebocoran Resource) ────────────
// KAPAN TERJADI: Saat objek yang mengimplementasikan IDisposable (koneksi DB, file stream, HttpClient)
//               tidak di-dispose setelah digunakan.
// AKIBAT: Memory leak, kehabisan koneksi database, file tidak bisa dibuka karena masih "terkunci".
// SOLUSI: Selalu gunakan `using` statement/declaration untuk resource yang mengimplementasikan IDisposable.
Console.WriteLine("--- 2. CONNECTION LEAK PREVENTION ---");
Console.WriteLine("Kebocoran resource terjadi jika file/koneksi DB yang dibuka tidak ditutup.");

// ✅ BENAR: `using (...)` menjamin Dispose() otomatis dipanggil di akhir scope,
// BAHKAN jika terjadi exception di dalam blok kode. Ini menggunakan pola RAII (Resource Acquisition Is Initialization).
using (var mockConnection = new MockDatabaseConnection())
{
    mockConnection.OpenConnection();
    // Proses query data...
} // Di sini, Dispose() otomatis dipanggil dan koneksi ditutup dengan bersih.

Console.WriteLine("Koneksi berhasil ditutup otomatis melalui Dispose/using block.\n");

// ── BUG #3: Async Deadlock ────────────────────────────────────────────
// KAPAN TERJADI: Saat kode sinkron (non-async) memaksa menunggu task asinkron selesai
//               menggunakan `.Result` atau `.Wait()` di konteks thread tunggal (ASP.NET classic, WPF, WinForms).
// 
// PENJELASAN DEADLOCK:
//   Thread UI/ASP.NET: memanggil asyncTask.Result → memblokir (menunggu)
//   Task asinkron: selesai, mencoba lanjut di thread asal → thread asal sudah diblokir!
//   → Deadlock: keduanya saling menunggu selamanya.
//
// SOLUSI: "Async All The Way" — jika satu method async, semua callernya juga harus async.
Console.WriteLine("--- 3. ASYNC DEADLOCK PREVENTION ---");
Console.WriteLine("[Info] Menjalankan operasi asinkron secara aman...");

// ❌ HINDARI DI ASP.NET/WPF: var result = FetchDataFromNetworkAsync().Result;
// ❌ HINDARI DI ASP.NET/WPF: FetchDataFromNetworkAsync().Wait();

// ✅ BENAR: Selalu gunakan `await` → thread tidak diblokir, menghindari deadlock
var data = await FetchDataFromNetworkAsync();
Console.WriteLine($"Hasil data: {data}");

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Debugging Selesai!");
Console.WriteLine("============================================================");

// =========================================================================
// HELPER CLASSES & METHODS
// =========================================================================

/// <summary>
/// FUNGSI METHOD: Simulasi pemanggilan API jaringan yang asinkron.
/// Didesain async agar dapat didemonstrasikan dengan `await` yang aman.
/// </summary>
async Task<string> FetchDataFromNetworkAsync()
{
    await Task.Delay(500); // Simulasi HTTP request ke server eksternal (~500ms)
    return "Response Sukses dari Server API";
}

/// <summary>
/// TUJUAN CLASS: Model produk sederhana untuk simulasi NullReferenceException.
/// Properti Category bertipe nullable (Category?) untuk merepresentasikan hubungan opsional.
/// </summary>
public class Product
{
    public string    Name     { get; set; } = string.Empty;
    public Category? Category { get; set; }  // Nullable — bisa null jika produk belum dikategorikan
}

/// <summary>
/// TUJUAN CLASS: Data kategori produk.
/// </summary>
public class Category
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// TUJUAN CLASS:
/// Simulasi objek koneksi database yang mengimplementasikan IDisposable untuk demonstrasi resource management.
/// 
/// POLA IDisposable:
/// Setiap objek yang mengelola resource tidak terkelola (unmanaged resources) seperti koneksi database,
/// file handle, atau soket jaringan WAJIB mengimplementasikan IDisposable dan membebaskan resource di Dispose().
/// </summary>
public class MockDatabaseConnection : IDisposable
{
    private bool _isOpen;

    /// <summary>
    /// FUNGSI METHOD: Membuka koneksi ke database.
    /// Di dunia nyata: mengambil koneksi dari connection pool SQL Server.
    /// </summary>
    public void OpenConnection()
    {
        _isOpen = true;
        Console.WriteLine("[DB Connection] Koneksi dibuka.");
    }

    /// <summary>
    /// FUNGSI METHOD: Membebaskan resource (menutup koneksi).
    /// Dipanggil otomatis oleh `using` statement saat keluar dari scope.
    /// 
    /// BEST PRACTICE: Selalu periksa `if (_isOpen)` sebelum menutup untuk menghindari double-dispose.
    /// </summary>
    public void Dispose()
    {
        if (_isOpen)
        {
            _isOpen = false;
            Console.WriteLine("[DB Connection] Koneksi ditutup (Disposed).");
        }
    }
}
