// ============================================================
// Nama File: LifetimeServices.cs — Demo DI Service Lifetime di ASP.NET Core
// Folder: 16-Framework/Source/Services/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendefinisikan interface dan implementasi service untuk mendemonstrasikan
//      perbedaan ketiga jenis lifetime di ASP.NET Core Dependency Injection Container.
//    - Mengapa Diperlukan: Memahami DI Lifetime adalah salah satu topik teknis wajib dalam interview backend .NET.
//      Salah memilih lifetime dapat menyebabkan bug kritis (misal: Scoped service diakses dari Singleton — Captive Dependency).
//    - Hubungan File: Ketiga interface ini didaftarkan di Program.cs dengan lifetime berbeda dan digunakan di DemoController.
//    - Jika Dihapus: Tidak ada demonstrasi fungsional perbedaan DI Lifetime.
// ============================================================

namespace TesBackendNet.Framework.Services;

// ── PENJELASAN KETIGA LIFETIME ─────────────────────────────────
// 1. TRANSIENT (AddTransient):
//    - Instance BARU dibuat SETIAP KALI service ini di-inject/diminta dari DI Container.
//    - Jika satu request memanggil DI dua kali, akan ada 2 instance berbeda (GUID berbeda).
//    - Kapan digunakan: Service ringan, stateless, dan aman digunakan secara bersamaan (thread-safe).
//    - Contoh nyata: Formatter, Parser, Calculator.
//
// 2. SCOPED (AddScoped):
//    - Instance BARU dibuat SEKALI per HTTP Request, lalu dibagikan di seluruh request tersebut.
//    - Jika satu request memanggil DI dua kali, keduanya mendapat INSTANCE YANG SAMA (GUID sama).
//    - Saat request berakhir, instance dihancurkan (disposed).
//    - Kapan digunakan: DbContext (EF Core), layanan yang perlu state dalam satu request.
//    - Contoh nyata: AppDbContext, Unit of Work.
//
// 3. SINGLETON (AddSingleton):
//    - Hanya SATU instance yang dibuat SELAMA APLIKASI berjalan.
//    - Semua request dan semua class berbagi instance yang sama persis.
//    - Kapan digunakan: Service yang mahal dibuat ulang (koneksi cache, konfigurasi).
//    - BAHAYA: Jika menyimpan state mutable (dapat berubah), dapat menyebabkan race condition pada concurrent request.
//    - Contoh nyata: IMemoryCache, HttpClient, konfigurasi aplikasi.

/// <summary>
/// TUJUAN INTERFACE: Kontrak untuk service yang didaftarkan dengan lifetime Transient.
/// </summary>
public interface ITransientService
{
    /// <summary>
    /// FUNGSI METHOD: Mengembalikan GUID unik yang dibuat saat instansiasi service.
    /// KEGUNAAN DEMO: Jika GUID berubah antar pemanggilan dalam satu request, berarti instance baru dibuat (Transient terbukti).
    /// </summary>
    Guid GetOperationId();
}

/// <summary>
/// TUJUAN INTERFACE: Kontrak untuk service yang didaftarkan dengan lifetime Scoped.
/// </summary>
public interface IScopedService
{
    /// <summary>Mengembalikan GUID yang sama selama satu HTTP Request berlangsung.</summary>
    Guid GetOperationId();
}

/// <summary>
/// TUJUAN INTERFACE: Kontrak untuk service yang didaftarkan dengan lifetime Singleton.
/// </summary>
public interface ISingletonService
{
    /// <summary>Mengembalikan GUID yang selalu sama di semua request sepanjang aplikasi berjalan.</summary>
    Guid GetOperationId();
}

/// <summary>
/// TUJUAN CLASS:
/// Implementasi tunggal yang mengimplementasikan ketiga interface sekaligus untuk simplifikasi demo.
/// 
/// PRINSIP UTAMA:
/// GUID dibuat sekali saat constructor dipanggil (`_operationId = Guid.NewGuid()`).
/// - Jika class ini didaftarkan sebagai Transient: Constructor dipanggil ulang setiap inject → GUID selalu berbeda.
/// - Jika didaftarkan sebagai Scoped: Constructor dipanggil sekali per request → GUID sama dalam request.
/// - Jika didaftarkan sebagai Singleton: Constructor dipanggil sekali saat startup → GUID sama selamanya.
/// </summary>
public class LifetimeDemoService : ITransientService, IScopedService, ISingletonService
{
    // GUID dibuat SATU KALI saat class ini diinstansiasi oleh DI Container
    private readonly Guid _operationId = Guid.NewGuid();

    /// <summary>
    /// Mengembalikan Operation ID yang dibuat saat instance ini lahir.
    /// </summary>
    public Guid GetOperationId() => _operationId;
}
