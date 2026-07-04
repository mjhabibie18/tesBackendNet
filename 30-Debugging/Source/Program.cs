// ============================================================
// Program.cs — Demo Debugging & Common Bugs Solutions
// ============================================================

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🔍 DEMO DEBUGGING & SOLUSI BUGS UMUM DI C# / .NET");
Console.WriteLine("============================================================\n");

// ── 1. Solusi NullReferenceException ────────────────────────
Console.WriteLine("--- 1. NULLREFERENCEEXCEPTION PREVENTION ---");
Product? dummyProduct = null;

try
{
    // ❌ AKAN CRASH: Mengakses properti objek yang bernilai null secara langsung
    Console.WriteLine($"[Dirty] Nama Kategori: {dummyProduct!.Category.Name}");
}
catch (NullReferenceException)
{
    Console.WriteLine("[Dirty] Terjadi NullReferenceException! Program crash jika tidak ditangkap.");
}

// ✅ AMAN: Menggunakan null-conditional (?.) dan null-coalescing (??)
string categoryName = dummyProduct?.Category?.Name ?? "Kategori Tidak Tersedia";
Console.WriteLine($"[Clean] Nama Kategori: {categoryName}");
Console.WriteLine();

// ── 2. Solusi Connection Leak (Kebocoran Resource) ──────────
Console.WriteLine("--- 2. CONNECTION LEAK PREVENTION ---");
Console.WriteLine("Kebocoran resource terjadi jika file/koneksi DB yang dibuka tidak ditutup.");

// ✅ AMAN: Menggunakan scope 'using var' atau 'using (resource)'
// Resource akan di-dispose secara otomatis saat keluar dari scope method,
// bahkan jika terjadi exception di tengah-tengah eksekusi.
using (var mockConnection = new MockDatabaseConnection())
{
    mockConnection.OpenConnection();
    // Proses query data...
} // Di titik ini, metode Dispose() otomatis dipanggil, menutup koneksi.

Console.WriteLine("Koneksi berhasil ditutup otomatis melalui Dispose/using block.\n");

// ── 3. Solusi Async Deadlock ─────────────────────────────────
Console.WriteLine("--- 3. ASYNC DEADLOCK PREVENTION ---");
Console.WriteLine("[Info] Menjalankan operasi asinkron secara aman...");

// ❌ HINDARI: var result = FetchDataFromNetworkAsync().Result; 
// Penggunaan .Result atau .Wait() memblokir thread sinkron dan memicu deadlock di MVC/WPF context.

// ✅ SOLUSI: Selalu gunakan await untuk mengeksekusi operasi asinkron (Async all the way)
var data = await FetchDataFromNetworkAsync();
Console.WriteLine($"Hasil data: {data}");

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Debugging Selesai!");
Console.WriteLine("============================================================");

// =========================================================================
// ── HELPER CLASSES & METHODS ─────────────────────────────────────────────
// =========================================================================

public class Product
{
    public string Name { get; set; } = string.Empty;
    public Category? Category { get; set; }
}

public class Category
{
    public string Name { get; set; } = string.Empty;
}

// Simulasi objek database connection yang harus di-dispose
public class MockDatabaseConnection : IDisposable
{
    private bool _isOpen;

    public void OpenConnection()
    {
        _isOpen = true;
        Console.WriteLine("[DB Connection] Koneksi dibuka.");
    }

    public void Dispose()
    {
        if (_isOpen)
        {
            _isOpen = false;
            Console.WriteLine("[DB Connection] Koneksi ditutup (Disposed).");
        }
    }
}

static async Task<string> FetchDataFromNetworkAsync()
{
    await Task.Delay(500); // Simulasi request HTTP asinkron
    return "Response Sukses dari Server API";
}
