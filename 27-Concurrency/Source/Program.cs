// ============================================================
// Nama File: Program.cs — Demo Concurrency & Async Programming
// Folder: 27-Concurrency/Source/
// ============================================================
// 1. PENJELASAN FOLDER (Concurrency):
//    - Tujuan: Menerapkan konsep pemrograman asinkron, manajemen multithreading, mitigasi race conditions, dan pembatalan operasi di .NET.
//    - Kapan Digunakan: Ketika mengembangkan aplikasi high-performance yang melayani ribuan request konkuren tanpa memblokir thread pool utama.
//    - Hubungan: Memperjelas dasar-dasar di balik penulisan metode asynchronous (`async/await`) yang digunakan di Controller dan Service modul lainnya.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyajikan 4 skenario simulasi konkurensi: Asinkron vs Sinkron, Race Condition, Async Lock, dan Cancellation Token.
//    - Mengapa Diperlukan: Pemahaman konkurensi sangat krusial bagi backend developer agar dapat menghindari bug aneh (seperti corrupt data) saat server melayani akses simultan.
//    - Jika Dihapus: Modul ini kehilangan kode eksekusi utama, sehingga tidak dapat dijalankan untuk mendemonstrasikan thread-safety.
// ============================================================

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

Console.Clear();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("============================================================");
Console.WriteLine("⚡ DEMO CONCURRENCY & ASYNC PROGRAMMING (.NET 8/9)");
Console.WriteLine("============================================================");
Console.ResetColor();

// ── 1. ASYNC NON-BLOCKING VS SYNC BLOCKING ──────────────────────────────────
// Kasus: Menjalankan dua buah tugas independen yang masing-masing membutuhkan waktu 1 detik.
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("\n--- 1. ASYNC NON-BLOCKING VS SYNC BLOCKING ---");
Console.ResetColor();

var sw = Stopwatch.StartNew();
Console.WriteLine("[Sync] Memulai tugas pemblokiran...");
PerformTaskSync();
PerformTaskSync();
// Total waktu eksekusi adalah akumulasi: 1 detik + 1 detik = ~2 detik.
Console.WriteLine($"[Sync] Selesai berurutan dalam {sw.ElapsedMilliseconds} ms.\n");

sw.Restart();
Console.WriteLine("[Async] Memulai tugas non-blocking secara paralel...");
// Tugas dimulai secara asinkron di Thread Pool. Kontrol langsung dikembalikan tanpa menunggu tugas selesai.
Task task1 = PerformTaskAsync("Tugas-A", 1500);
Task task2 = PerformTaskAsync("Tugas-B", 1000);

// Task.WhenAll: Menunggu kedua task asinkron selesai secara non-blocking.
// Karena berjalan secara konkuren (bersamaan), total waktu yang dibutuhkan hanyalah durasi dari task terlama (1.5 detik).
await Task.WhenAll(task1, task2);
Console.WriteLine($"[Async] Selesai bersamaan dalam {sw.ElapsedMilliseconds} ms.\n");

// ── 2. RACE CONDITION & SYNCHRONIZATION (THREAD SAFETY) ─────────────────────
// Kasus: 100.000 iterasi penambahan nilai (increment) yang dijalankan oleh banyak thread secara paralel.
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("--- 2. RACE CONDITION & INTERLOCKED ---");
Console.ResetColor();

int unsafeCounter = 0;
int safeCounter = 0;
const int iterations = 100000;

// Parallel.For: Menjalankan perulangan terdistribusi pada beberapa thread CPU secara simultan.
Parallel.For(0, iterations, _ =>
{
    // A. Tidak Aman (Race Condition):
    // Operasi 'unsafeCounter++' sebenarnya terdiri dari 3 instruksi CPU tingkat rendah: Read, Modify, Write.
    // Ketika banyak thread melakukan Read secara bersamaan sebelum Write selesai, nilai increment akan hilang (lost update).
    unsafeCounter++; 

    // B. Aman (Interlocked - Atomic Operation):
    // Menggunakan instruksi CPU tingkat rendah (assembly) LOCK yang menjamin operasi Read-Modify-Write diselesaikan
    // secara atomik (sebagai satu kesatuan utuh) tanpa interupsi dari thread lain.
    Interlocked.Increment(ref safeCounter);
});

Console.WriteLine($"Parallel Increment {iterations}x:");
Console.WriteLine($"- Unsafe Counter (Race Condition) = {unsafeCounter} (Kurang dari {iterations} karena rebutan resource)");
Console.WriteLine($"- Safe Counter (Interlocked)      = {safeCounter} (Tepat {iterations}!)");
Console.WriteLine();

// ── 3. ASYNC LOCK MENGGUNAKAN SEMAPHORESLIM ──────────────────────────────────
// Kasus: Dua request mencoba menulis data ke database pada saat bersamaan.
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("--- 3. ASYNC LOCK (SEMAPHORESLIM) ---");
Console.ResetColor();

var apiDemo = new ThreadSafeApiDemo();
var writeTask1 = apiDemo.WriteDataAsync("Request-1");
var writeTask2 = apiDemo.WriteDataAsync("Request-2");
await Task.WhenAll(writeTask1, writeTask2);
Console.WriteLine();

// ── 4. GRACEFUL CANCELLATION (CANCELLATION TOKEN) ────────────────────────────
// Kasus: Client membatalkan pemanggilan API yang lambat (misalnya menutup tab browser sebelum download selesai).
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("--- 4. CANCELLATION TOKEN ---");
Console.ResetColor();

// CancellationTokenSource memicu pembatalan token.
var cts = new CancellationTokenSource();

// Mensimulasikan pembatalan otomatis oleh sistem setelah 1 detik.
cts.CancelAfter(1000);

try
{
    Console.WriteLine("[Client] Mengirim request asinkron yang memakan waktu lama...");
    // Meneruskan token pembatalan ke method asinkron.
    await LongRunningApiCallAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Tangkap exception pembatalan secara khusus.
    Console.WriteLine("[Server] SUKSES: Operasi asinkron berhasil dihentikan (Canceled) untuk menghemat resource server.");
}

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Concurrency Selesai!");
Console.WriteLine("============================================================");


// =========================================================================
// ── HELPER METHODS (IMPLEMENTASI DETAIL) ─────────────────────────────────
// =========================================================================

/// <summary>
/// FUNGSI METHOD: Memblokir thread eksekusi saat ini selama 1 detik secara sinkron.
/// ALASAN IMPLEMENTASI: Menggunakan Thread.Sleep yang menahan thread aktif dari pool (sangat buruk untuk scalability web server).
/// </summary>
static void PerformTaskSync()
{
    Thread.Sleep(1000);
}

/// <summary>
/// FUNGSI METHOD: Menunda proses secara asinkron tanpa memblokir thread.
/// PARAMETER:
///  - name: Nama identitas tugas.
///  - delayMs: Durasi penundaan dalam milidetik.
/// NILAI KEMBALIAN: Task (mewakili operasi asinkron yang sedang berjalan).
/// </summary>
static async Task PerformTaskAsync(string name, int delayMs)
{
    // Task.Delay melepaskan thread kembali ke pool selama masa tunggu agar dapat melayani pekerjaan lain.
    await Task.Delay(delayMs);
    Console.WriteLine($"  -> {name} selesai.");
}

/// <summary>
/// FUNGSI METHOD: Simulasi unduhan data asinkron berdurasi lama dengan dukungan pembatalan.
/// PARAMETER:
///  - ct: CancellationToken untuk memantau sinyal pembatalan dari client.
/// </summary>
static async Task LongRunningApiCallAsync(CancellationToken ct)
{
    for (int i = 1; i <= 5; i++)
    {
        // Mengecek secara proaktif sebelum memulai part berikutnya.
        // Jika token telah dibatalkan, langsung melempar OperationCanceledException secara instan.
        ct.ThrowIfCancellationRequested();

        // Mengirimkan token ke sub-task (Task.Delay) agar penundaan asinkron dibatalkan seketika.
        await Task.Delay(500, ct);
        Console.WriteLine($"  -> Mengunduh data part {i}/5...");
    }
}

// ── Class Demo SemaphoreSlim ──────────────────────────────────────────
/// <summary>
/// TUJUAN CLASS:
/// Menyediakan lingkungan penulisan data yang aman dari konflik multi-thread (*Thread-Safe*).
/// 
/// ALASAN MENGGUNAKAN SEMAPHORESLIM:
/// Konstruksi keyword 'lock' bawaan C# bersifat memblokir thread (*synchronous blocking*) dan tidak mendukung keyword `await` di dalamnya.
/// SemaphoreSlim(1, 1) bertindak sebagai asinkron lock (*async lock*). Ia membatasi akses ke blok kode kritis maksimal 1 thread
/// secara non-blocking. Thread lain yang mengantri dibebaskan untuk melakukan aktivitas lain.
/// </summary>
public class ThreadSafeApiDemo
{
    // Inisialisasi: kapasitas awal 1 slot akses, kapasitas maksimal 1 slot akses.
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// FUNGSI METHOD: Menulis data ke database secara eksklusif (satu per satu).
    /// PARAMETER: requestName (Identitas request pengirim).
    /// </summary>
    public async Task WriteDataAsync(string requestName)
    {
        Console.WriteLine($"[{requestName}] Menunggu giliran menulis database...");
        
        // Memperoleh slot lock secara asinkron. Jika slot terisi, thread akan ditangguhkan tanpa memblokir.
        await _semaphore.WaitAsync(); 
        try
        {
            Console.WriteLine($"[{requestName}] LOCK DIDAPATKAN. Sedang menulis...");
            await Task.Delay(1000); // Simulasi proses I/O tulis database
            Console.WriteLine($"[{requestName}] Penulisan selesai. Melepas lock.");
        }
        finally
        {
            // PENTING: Selalu panggil Release() di blok 'finally' untuk menjamin 
            // slot lock dilepaskan kembali, mencegah terjadinya deadlock jika proses try mengalami error.
            _semaphore.Release(); 
        }
    }
}
