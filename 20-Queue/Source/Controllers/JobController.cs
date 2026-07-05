// ============================================================
// Nama File: JobController.cs — Controller Pemicu Background Jobs (Hangfire)
// Folder: 20-Queue/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Queue):
//    - Tujuan: Mendemonstrasikan cara mendelegasikan pekerjaan berat atau lambat (seperti pengiriman email, pembuatan laporan)
//      ke background worker agar HTTP request dapat selesai segera tanpa memblokir pengguna.
//    - Kapan Digunakan: Saat ada tugas yang memakan waktu lama (>500ms) yang tidak perlu hasilnya langsung ditampilkan ke pengguna.
//    - Hubungan: Menggunakan Hangfire (library background job) dan IEmailService untuk demonstrasi task berbeda.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengekspos 4 endpoint yang memicu 3 jenis background job berbeda di Hangfire.
//    - Mengapa Diperlukan: Menunjukkan perbedaan Fire-and-Forget, Delayed Job, dan Recurring Job yang masing-masing cocok
//      untuk skenario produksi yang berbeda.
//    - Jika Dihapus: Tidak ada demonstrasi background processing pada modul Queue.
// ============================================================

using Hangfire;
using Microsoft.AspNetCore.Mvc;
using TesBackendNet.Queue.Services;

namespace TesBackendNet.Queue.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang menerima HTTP request dan mendelegasikan pekerjaan ke Hangfire Background Job Queue.
/// 
/// KONSEP UTAMA (Mengapa Background Job?):
/// HTTP Request memiliki timeout (biasanya 30 detik). Jika endpoint melakukan operasi lambat 
/// (kirim 1000 email, generate laporan PDF besar), pengguna akan menunggu lama atau bahkan timeout.
/// Dengan background job: HTTP request langsung dikembalikan (202 Accepted) dan pekerjaan 
/// dilanjutkan di background tanpa pengguna perlu menunggu.
/// 
/// TIGA JENIS JOB DI HANGFIRE:
/// 1. Fire-and-Forget: Dijalankan sekali, sesegera mungkin, di background.
/// 2. Delayed Job: Dijalankan sekali setelah jeda waktu tertentu.
/// 3. Recurring Job: Dijalankan berulang kali sesuai jadwal CRON expression.
/// </summary>
[ApiController]
[Route("api/jobs")]
public class JobController : ControllerBase
{
    private readonly IBackgroundJobClient  _backgroundJobClient;
    private readonly IRecurringJobManager  _recurringJobManager;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan Hangfire job client dan manager dari DI Container.
    /// Keduanya didaftarkan secara otomatis oleh `services.AddHangfire(...)` di Program.cs.
    /// </summary>
    public JobController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    // ================================================================
    // 1. FIRE-AND-FORGET JOB
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Memicu pengiriman email selamat datang di background setelah registrasi pengguna.
    /// PARAMETER: email, username (data pengguna baru).
    /// NILAI KEMBALIAN: HTTP 202 Accepted — request diterima, pekerjaan sedang berjalan di background.
    /// 
    /// ALASAN MENGGUNAKAN HTTP 202 (ACCEPTED):
    /// HTTP 200 (OK) berarti pekerjaan selesai. HTTP 202 (Accepted) berarti request sudah diterima 
    /// tetapi pekerjaan masih diproses. Ini lebih jujur secara semantis untuk background job.
    /// 
    /// CARA KERJA `_backgroundJobClient.Enqueue<IEmailService>`:
    /// Hangfire menyimpan informasi method yang akan dipanggil (bukan hasil eksekusinya) ke database/storage.
    /// Worker process Hangfire mengambil job ini dari antrian dan mengeksekusinya secara independen dari HTTP request.
    /// </summary>
    [HttpPost("welcome")]
    public IActionResult TriggerWelcomeJob([FromQuery] string email, [FromQuery] string username)
    {
        // Enqueue: Masukkan job ke antrian untuk segera dieksekusi di background
        // Thread utama HTTP langsung melanjutkan ke baris return di bawah
        var jobId = _backgroundJobClient.Enqueue<IEmailService>(service => 
            service.SendWelcomeEmailAsync(email, username));

        return Accepted(new
        {
            Success = true,
            JobId   = jobId,          // ID ini dapat digunakan untuk melacak status job di dashboard Hangfire
            Message = "Request pendaftaran diterima. Email akan dikirim di background."
        });
    }

    // ================================================================
    // 2. DELAYED JOB
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Memicu pengiriman email invoice yang ditunda 10 detik setelah order dibuat.
    /// PARAMETER: orderId (ID pesanan), email (penerima invoice).
    /// 
    /// MENGAPA DELAYED?
    /// Skenario: Sistem perlu memberi waktu kepada proses lain untuk menyelesaikan data order 
    /// (misal: konfirmasi stok, finalisasi harga) sebelum invoice dikirim ke pelanggan.
    /// 
    /// `TimeSpan.FromSeconds(10)`: Membuat objek durasi 10 detik.
    /// Job ini TIDAK langsung dieksekusi, melainkan dijadwalkan 10 detik dari sekarang.
    /// </summary>
    [HttpPost("invoice/{orderId:int}")]
    public IActionResult TriggerInvoiceJob(int orderId, [FromQuery] string email)
    {
        // Schedule: Daftarkan job yang akan dieksekusi setelah 10 detik dari sekarang
        var jobId = _backgroundJobClient.Schedule<IEmailService>(
            service => service.SendInvoiceEmailAsync(email, orderId),
            TimeSpan.FromSeconds(10));  // Delay 10 detik

        return Accepted(new
        {
            Success = true,
            JobId   = jobId,
            Message = $"Order #{orderId} diproses. Invoice akan dikirim secara otomatis 10 detik dari sekarang."
        });
    }

    // ================================================================
    // 3. RECURRING JOB
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mendaftarkan job backup database yang berjalan berulang setiap menit.
    /// 
    /// CRON EXPRESSION:
    /// `Cron.Minutely` adalah ekuivalen dari CRON expression `* * * * *` (setiap menit).
    /// CRON adalah standar Unix untuk penjadwalan berulang: `Minute Hour Day Month DayOfWeek`.
    /// Contoh: `0 8 * * 1-5` = Jam 8 pagi setiap hari kerja (Senin-Jumat).
    /// 
    /// `"db-backup-every-minute"`: Job ID yang unik. Jika job dengan ID ini sudah ada, maka akan diperbarui (AddOrUpdate).
    /// </summary>
    [HttpPost("backup/register")]
    public IActionResult RegisterBackupJob()
    {
        // AddOrUpdate: Daftarkan job berulang. Jika ID sudah ada, update jadwalnya.
        _recurringJobManager.AddOrUpdate<IEmailService>(
            "db-backup-every-minute",          // Unique Job ID
            service => service.RunDatabaseBackup(), // Pekerjaan yang akan dieksekusi
            Cron.Minutely);                    // Jadwal: Setiap menit

        return Ok(new
        {
            Success = true,
            Message = "Job backup database rutin (setiap 1 menit) berhasil diaktifkan."
        });
    }

    /// <summary>
    /// FUNGSI METHOD: Menghentikan dan menghapus job backup database yang berulang.
    /// `RemoveIfExists`: Menghapus job jika ada, tidak melempar exception jika tidak ditemukan.
    /// </summary>
    [HttpDelete("backup/remove")]
    public IActionResult RemoveBackupJob()
    {
        _recurringJobManager.RemoveIfExists("db-backup-every-minute");
        return Ok(new { Success = true, Message = "Job backup database rutin berhasil dihentikan." });
    }
}
