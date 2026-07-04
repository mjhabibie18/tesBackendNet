using System.ComponentModel.DataAnnotations;
using TesBackendNet.RESTfulAPI.Models;

namespace TesBackendNet.RESTfulAPI.DTO;

public class ProductCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}

public class ProductUpdateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; }

    // ── HATEOAS Links ──────────────────────────────────────────
    // Menyimpan metadata link untuk navigasi client
    public List<LinkDto> Links { get; set; } = new();
}
