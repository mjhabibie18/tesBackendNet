// ============================================================
// Blog.cs — Model Parent untuk Demo Relasi
// ============================================================

namespace TesBackendNet.Database.Models;

public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    // Navigation Property: Satu Blog memiliki banyak Posts (1-to-Many)
    public List<Post> Posts { get; set; } = new();
}
