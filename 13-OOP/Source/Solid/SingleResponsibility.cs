// ============================================================
// Nama File: SingleResponsibility.cs — Demo SOLID: Single Responsibility Principle (SRP)
// Folder: 13-OOP/Source/Solid/
// ============================================================
// 1. PENJELASAN FOLDER (OOP/Solid):
//    - Tujuan: Menyediakan contoh nyata prinsip SOLID.
//    - Kapan Digunakan: Saat mendesain class-class agar masing-masing memiliki fokus tugas tunggal.
//    - Hubungan: Terkoneksi dengan kelas GoodProductService dan kelas bisnis lainnya dalam membagi peran.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Memperagakan pemisahan peran sebuah class manager yang gemuk (God Class) menjadi kelas-kelas khusus.
//    - Mengapa Diperlukan: Kode yang menumpuk banyak tugas dalam satu tempat sangat rentan rusak saat dimodifikasi dan sulit di-test.
//    - Hubungan File: Dipanggil oleh Program.cs pada saat menjalankan demo modul OOP.
// ============================================================

namespace TesBackendNet.OOP.Solid;

// ── BENTUK SALAH (Melanggar SRP) ─────────────────────────────
// Keterangan Pelanggaran:
// BadUserManager mencoba mengurus pendaftaran user, penulisan ke database, dan pengiriman email sekaligus.
// Jika format email berubah, class ini harus diubah. Jika skema database berubah, class ini juga harus diubah.
// Ini menyebabkan satu class memiliki banyak alasan untuk berubah (Multiple Reasons to Change).

/// <summary>
/// TUJUAN CLASS: Manager pengguna yang menanggung terlalu banyak tanggung jawab.
/// </summary>
public class BadUserManager
{
    /// <summary>
    /// FUNGSI METHOD: Membuat pengguna baru sekaligus menyimpan dan mengirim email secara langsung.
    /// </summary>
    public void CreateUser(string username)
    {
        Console.WriteLine($"Bad: User {username} dibuat.");
        Console.WriteLine("Bad: Menyimpan ke database...");
        Console.WriteLine("Bad: Mengirim email selamat datang...");
    }
}

// ── BENTUK BENAR (Memenuhi SRP) ──────────────────────────────
// Keterangan Perbaikan:
// Logika dipecah menjadi tiga kelas terpisah sesuai spesifikasinya.
// - DatabaseService: Fokus hanya pada operasi I/O database.
// - EmailNotificationService: Fokus hanya pada pengiriman surel/email.
// - UserService: Orkestrator bisnis yang memanggil kedua service tersebut.

/// <summary>
/// TUJUAN CLASS: Mengelola operasi tulis data ke penyimpanan database.
/// </summary>
public class DatabaseService
{
    /// <summary>
    /// FUNGSI METHOD: Menyimpan data teks ke database.
    /// PARAMETER: data (string konten yang akan disimpan).
    /// </summary>
    public void Save(string data)
    {
        Console.WriteLine($"[Database Service] Menyimpan data: {data}");
    }
}

/// <summary>
/// TUJUAN CLASS: Mengelola notifikasi surat elektronik (email).
/// </summary>
public class EmailNotificationService
{
    /// <summary>
    /// FUNGSI METHOD: Mengirimkan email selamat datang ke pengguna baru.
    /// PARAMETER: username (identitas pengguna tujuan).
    /// </summary>
    public void SendWelcomeEmail(string username)
    {
        Console.WriteLine($"[Email Service] Mengirim welcome email ke {username}");
    }
}

/// <summary>
/// TUJUAN CLASS: Orkestrator alur bisnis pembuatan pengguna baru.
/// DEPENDENCY: DatabaseService dan EmailNotificationService disuntikkan lewat konstruktor.
/// </summary>
public class UserService
{
    private readonly DatabaseService _dbService;
    private readonly EmailNotificationService _emailService;

    /// <summary>
    /// CONSTRUCTOR (Dependency Injection):
    /// Menerima objek service luar yang sudah diinstansiasi agar tidak melakukan 'new' secara internal.
    /// </summary>
    public UserService(DatabaseService dbService, EmailNotificationService emailService)
    {
        _dbService = dbService;
        _emailService = emailService;
    }

    /// <summary>
    /// FUNGSI METHOD: Melakukan registrasi pengguna.
    /// PARAMETER: username (nama pengguna baru).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Menampilkan log pendaftaran di konsol.
    /// 2. Menyimpan data pengguna menggunakan DatabaseService.
    /// 3. Mengirimkan email selamat datang menggunakan EmailNotificationService.
    /// 
    /// BEST PRACTICE:
    /// Menerapkan SRP membuat unit testing menjadi sangat mudah. Kita bisa menguji
    /// fungsionalitas EmailNotificationService secara terpisah tanpa melibatkan Database.
    /// </summary>
    public void CreateUser(string username)
    {
        Console.WriteLine($"[User Service] Membuat user: {username}");
        _dbService.Save($"User {username}");
        _emailService.SendWelcomeEmail(username);
    }
}
