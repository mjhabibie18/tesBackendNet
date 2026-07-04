// ============================================================
// Post.cs — Model Child untuk Demo Relasi & Transactions
// ============================================================

namespace TesBackendNet.Database.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Views { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Foreign Key ke tabel Blogs
    public int BlogId { get; set; }
    
    // Navigation Property ke parent Blog
    public Blog Blog { get; set; } = null!;
}
