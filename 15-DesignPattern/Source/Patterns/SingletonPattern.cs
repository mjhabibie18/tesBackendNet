// ============================================================
// SingletonPattern.cs — Implementasi Singleton
// ============================================================
// Pola Singleton menjamin hanya ada SATU instance dari sebuah
// class di seluruh siklus hidup aplikasi.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

public sealed class DatabaseConfigManager
{
    private static DatabaseConfigManager? _instance;
    private static readonly object _lock = new();

    // Private constructor: mencegah pembuatan objek secara langsung menggunakan kata kunci 'new'
    private DatabaseConfigManager()
    {
        Console.WriteLine("[Singleton] Inisialisasi DatabaseConfigManager (Hanya Sekali!)");
        ConnectionString = "Server=localhost;Database=TesDb;Trusted_Connection=True;";
    }

    public string ConnectionString { get; set; }

    // Thread-safe Singleton menggunakan double-check locking
    public static DatabaseConfigManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new DatabaseConfigManager();
                }
            }
            return _instance;
        }
    }
}
