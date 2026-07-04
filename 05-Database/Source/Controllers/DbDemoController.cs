// ============================================================
// DbDemoController.cs — Controller Demo Query Database & Transactions
// ============================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TesBackendNet.Database.Data;
using TesBackendNet.Database.Models;

namespace TesBackendNet.Database.Controllers;

[ApiController]
[Route("api/database")]
public class DbDemoController : ControllerBase
{
    private readonly AppDbContext _context;

    public DbDemoController(AppDbContext context)
    {
        _context = context;
    }

    // ================================================================
    // 1. LINQ Queries Demo (Where, OrderBy, Include)
    // ================================================================
    
    [HttpGet("posts")]
    public async Task<IActionResult> GetPosts([FromQuery] string? search)
    {
        var query = _context.Posts
            .Include(p => p.Blog) // JOIN: Mengambil data parent Blog
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(p => p.Title.Contains(search));
        }

        var posts = await query
            .OrderByDescending(p => p.Views) // Mengurutkan berdasarkan views terbanyak
            .ToListAsync();

        return Ok(posts);
    }

    // ================================================================
    // 2. Aggregate Queries Demo (Sum, Average, Max, GroupBy)
    // ================================================================
    
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalViews = await _context.Posts.SumAsync(p => p.Views);
        var avgViews = await _context.Posts.AverageAsync(p => p.Views);
        var maxViews = await _context.Posts.MaxAsync(p => p.Views);

        // Group By: Menghitung total post & views per Blog
        var blogStats = await _context.Posts
            .GroupBy(p => p.BlogId)
            .Select(g => new
            {
                BlogId = g.Key,
                TotalPosts = g.Count(),
                TotalViews = g.Sum(p => p.Views)
            })
            .ToListAsync();

        return Ok(new
        {
            TotalViews = totalViews,
            AverageViews = avgViews,
            MaximumViews = maxViews,
            BlogStats = blogStats
        });
    }

    // ================================================================
    // 3. Database Transaction Demo (Manual Rollback/Commit)
    // ================================================================
    
    [HttpPost("transaction-demo")]
    public async Task<IActionResult> RunTransactionDemo([FromQuery] bool triggerError)
    {
        // Memulai blok transaksi database manual
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // A. Simpan data Blog baru
            var newBlog = new Blog { Title = "Transaction Demo Blog", Url = "https://demo.com" };
            _context.Blogs.Add(newBlog);
            await _context.SaveChangesAsync(); // Men-generate ID baru

            // B. Simpan data Post baru yang terkait dengan Blog di atas
            var newPost = new Post
            {
                Title = "Post Transaksi",
                Content = "Isi post transaksi...",
                Views = 10,
                BlogId = newBlog.Id
            };
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            // Pemicu error untuk mensimulasikan kegagalan dan rollback
            if (triggerError)
            {
                throw new InvalidOperationException("Pemicu error transaksi aktif. Transaksi dibatalkan (Rollback)!");
            }

            // C. Commit jika semua operasi berhasil disimpan tanpa error
            await transaction.CommitAsync();
            
            return Ok(new { Success = true, Message = "Transaksi berhasil dilakukan (Commit)." });
        }
        catch (Exception ex)
        {
            // D. Rollback: Membatalkan seluruh query di atas sehingga database tetap bersih
            await transaction.RollbackAsync();
            
            return BadRequest(new { Success = false, Message = "Transaksi gagal dan di-rollback.", Error = ex.Message });
        }
    }
}
