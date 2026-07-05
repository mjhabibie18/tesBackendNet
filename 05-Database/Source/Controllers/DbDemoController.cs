// ============================================================
// Nama File: DbDemoController.cs — Controller Demo Query Database & Transactions
// Folder: 05-Database/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Database):
//    - Tujuan: Mendemonstrasikan kueri database SQL melalui EF Core LINQ: SELECT, WHERE, JOIN, ORDER BY,
//      fungsi agregat (SUM, AVG, MAX), GROUP BY, dan transaksi database (BEGIN/COMMIT/ROLLBACK).
//    - Kapan Digunakan: Saat membangun fitur laporan, statistik, atau operasi tulis yang memerlukan atomisitas data.
//    - Hubungan: Memanggil AppDbContext.cs untuk semua interaksi database.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengekspos 3 endpoint demonstrasi yang menunjukkan kueri relasional dan manajemen transaksi.
//    - Mengapa Diperlukan: Memberikan contoh nyata pola kueri database yang umum digunakan dalam aplikasi backend produksi.
//    - Jika Dihapus: Tidak ada endpoint yang dapat digunakan untuk menguji fungsionalitas database pada modul ini.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.Database.Data;
using TesBackendNet.Database.Models;

namespace TesBackendNet.Database.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang menampilkan penggunaan LINQ-to-SQL melalui EF Core untuk 3 kategori operasi database:
/// 1. Kueri dengan JOIN dan filter dinamis.
/// 2. Kueri agregat (SUM, AVG, MAX, GROUP BY).
/// 3. Transaksi database manual dengan COMMIT dan ROLLBACK.
/// </summary>
[ApiController]
[Route("api/database")]
public class DbDemoController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan AppDbContext untuk seluruh operasi database.
    /// </summary>
    public DbDemoController(AppDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // 1. LINQ Queries Demo (Where, OrderBy, Include/JOIN)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mengambil daftar artikel (Post) dengan filter teks dan JOIN ke tabel Blog induknya.
    /// PARAMETER: search (opsional — kata kunci pencarian pada judul artikel).
    /// NILAI KEMBALIAN: Task<IActionResult> — HTTP 200 OK berisi daftar Post beserta data Blog terkait.
    /// 
    /// ALUR EKSEKUSI LINQ:
    /// 1. `_context.Posts` → Sumber data dari tabel Posts.
    /// 2. `.Include(p => p.Blog)` → Menghasilkan SQL LEFT JOIN ke tabel Blogs untuk mengambil data induk.
    /// 3. `.Where(p => !p.IsDeleted)` → Filter soft delete (SQL: WHERE IsDeleted = 0).
    /// 4. `if (!string.IsNullOrEmpty(search)) query = query.Where(...)` → Filter opsional, hanya ditambahkan jika ada kata kunci.
    /// 5. `.OrderByDescending(p => p.Views)` → SQL: ORDER BY Views DESC.
    /// 6. `.ToListAsync()` → Mengeksekusi semua operasi di atas menjadi satu query SQL ke database.
    /// 
    /// BEST PRACTICE:
    /// Membangun IQueryable secara bertahap (deferred execution) sebelum memanggil ToListAsync() di akhir 
    /// memastikan hanya satu query yang dikirim ke database, bukan banyak query (menghindari N+1 problem).
    /// </summary>
    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts([FromQuery] string? search)
    {
        // Membangun kueri secara bertahap (deferred execution — belum dieksekusi ke DB)
        var query = _context.Posts
            .Include(p => p.Blog)         // SQL JOIN: Sertakan data Blog induk
            .Where(p => !p.IsDeleted);    // SQL WHERE: Hanya tampilkan post yang belum dihapus

        // Filter pencarian dinamis (hanya ditambahkan jika parameter search tidak kosong)
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Title.Contains(search));
        }

        // Eksekusi akhir: semua kondisi di atas dikompilasi menjadi satu query SQL ke database
        var posts = await query
            .OrderByDescending(p => p.Views) // Urutkan: Post paling populer di paling atas
            .ToListAsync();

        return Ok(posts);
    }

    // ================================================================
    // 2. Aggregate Queries Demo (Sum, Average, Max, GroupBy)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mengambil statistik agregat dari seluruh tabel Posts.
    /// NILAI KEMBALIAN: Task<IActionResult> — HTTP 200 OK berisi objek statistik yang terstruktur.
    /// 
    /// FUNGSI AGREGAT YANG DIDEMONSTRASIKAN:
    /// - `SumAsync(p => p.Views)`: SQL: SELECT SUM(Views) FROM Posts.
    /// - `AverageAsync(p => p.Views)`: SQL: SELECT AVG(Views) FROM Posts.
    /// - `MaxAsync(p => p.Views)`: SQL: SELECT MAX(Views) FROM Posts.
    /// 
    /// PENJELASAN GROUP BY DENGAN LINQ:
    /// `.GroupBy(p => p.BlogId)`: SQL: GROUP BY BlogId
    /// `.Select(g => new {...})`: Mengambil aggregate per group:
    ///   - `g.Key` = nilai BlogId yang menjadi dasar pengelompokan.
    ///   - `g.Count()` = SQL: COUNT(*) per group.
    ///   - `g.Sum(p => p.Views)` = SQL: SUM(Views) per group.
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        // Fungsi agregat skalar — dieksekusi sebagai query terpisah, bukan satu query
        var totalViews = await _context.Posts.SumAsync(p => p.Views);
        var avgViews   = await _context.Posts.AverageAsync(p => p.Views);
        var maxViews   = await _context.Posts.MaxAsync(p => p.Views);

        // Group By: Menghitung total post & views per Blog — dieksekusi sebagai satu kueri SQL kompleks
        var blogStats = await _context.Posts
            .GroupBy(p => p.BlogId)           // Kelompokkan baris berdasarkan BlogId
            .Select(g => new                   // Pilih data agregat dari setiap kelompok
            {
                BlogId     = g.Key,            // Nilai pengelompokan (BlogId)
                TotalPosts = g.Count(),        // Jumlah post dalam kelompok ini
                TotalViews = g.Sum(p => p.Views) // Total views dari semua post dalam kelompok
            })
            .ToListAsync();

        return Ok(new
        {
            TotalViews    = totalViews,
            AverageViews  = avgViews,
            MaximumViews  = maxViews,
            BlogStats     = blogStats
        });
    }

    // ================================================================
    // 3. Database Transaction Demo (Manual Rollback/Commit)
    // ================================================================
    
    /// <summary>
    /// FUNGSI METHOD: Mendemonstrasikan transaksi database manual dengan skenario sukses dan gagal.
    /// PARAMETER: triggerError (bool) — jika true, sengaja memicu error untuk mensimulasikan ROLLBACK.
    /// 
    /// KONSEP TRANSAKSI DATABASE (ACID):
    /// - Atomicity: Semua operasi berhasil atau tidak sama sekali (all or nothing).
    /// - Consistency: Database selalu dalam keadaan valid setelah transaksi.
    /// - Isolation: Transaksi yang berjalan paralel tidak saling mengganggu.
    /// - Durability: Perubahan yang di-commit bersifat permanen.
    /// 
    /// ALUR EKSEKUSI:
    /// 1. `BeginTransactionAsync()` → Memulai blok transaksi. Semua operasi di dalamnya bersifat sementara.
    /// 2. Simpan Blog dan Post ke database sementara.
    /// 3. Jika `triggerError = true`, lempar exception sebelum commit.
    /// 4. `CommitAsync()` → Mengonfirmasi semua perubahan sementara ke database secara permanen.
    /// 5. Jika exception terjadi, blok `catch` memanggil `RollbackAsync()` untuk membatalkan semua perubahan sementara.
    /// 
    /// ALASAN `await using`:
    /// `await using var transaction` memastikan objek transaction di-dispose secara asinkron setelah blok try-catch selesai,
    /// melepaskan lock database yang ditahan selama transaksi berlangsung.
    /// </summary>
    [HttpPost("transaction-demo")]
    public async Task<IActionResult> RunTransactionDemo([FromQuery] bool triggerError)
    {
        // Memulai blok transaksi database manual — koneksi database "dikunci" selama ini
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // A. Simpan data Blog baru ke database (sementara, belum commit)
            var newBlog = new Blog { Title = "Transaction Demo Blog", Url = "https://demo.com" };
            _context.Blogs.Add(newBlog);
            await _context.SaveChangesAsync(); // Eksekusi INSERT ke DB, menghasilkan ID baru via IDENTITY

            // B. Simpan data Post baru yang bergantung pada ID Blog yang baru dibuat di atas
            var newPost = new Post
            {
                Title   = "Post Transaksi",
                Content = "Isi post transaksi...",
                Views   = 10,
                BlogId  = newBlog.Id  // Menggunakan ID yang baru saja di-generate oleh database
            };
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            // C. Simulasi kegagalan sistem di tengah proses transaksi
            if (triggerError)
            {
                // Exception ini akan ditangkap oleh blok catch di bawah dan memicu ROLLBACK
                throw new InvalidOperationException("Pemicu error transaksi aktif. Transaksi dibatalkan (Rollback)!");
            }

            // D. Commit: Menyelesaikan transaksi dan membuat semua perubahan sementara menjadi permanen di database
            await transaction.CommitAsync();
            
            return Ok(new { Success = true, Message = "Transaksi berhasil dilakukan (Commit)." });
        }
        catch (Exception ex)
        {
            // E. Rollback: Membatalkan SEMUA operasi database yang dilakukan sejak BeginTransactionAsync()
            // Baik Blog maupun Post yang sudah di-SaveChangesAsync() di dalam blok try akan DIBATALKAN.
            await transaction.RollbackAsync();
            
            return BadRequest(new { Success = false, Message = "Transaksi gagal dan di-rollback.", Error = ex.Message });
        }
    }
}
