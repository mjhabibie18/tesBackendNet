// ============================================================
// DTOs — Data Transfer Objects untuk Product
// ============================================================
// DTO adalah object yang digunakan untuk transfer data antara layer.
// BERBEDA dari Model/Entity yang merepresentasikan tabel database.
//
// Mengapa DTO?
//   1. Security: tidak expose field sensitif (IsDeleted, dll)
//   2. Flexibility: response bisa berbeda dari struktur database
//   3. Validation: validasi bisa dilakukan di level DTO
//   4. API Versioning: bisa buat v1, v2 DTO tanpa ubah Model
//
// File ini berisi semua DTO yang berkaitan dengan Product.
// Bisa juga dipisah per file, tapi untuk kesederhanaan digabung.
// ============================================================

using System.ComponentModel.DataAnnotations;

namespace TesBackendNet.CRUD.DTO;

// ── CREATE DTO ────────────────────────────────────────────────
// Digunakan untuk request POST /api/products
// Hanya berisi field yang boleh diisi oleh client

/// <summary>
/// DTO untuk membuat product baru.
/// Field yang TIDAK ada: Id (auto), CreatedAt (server), IsDeleted (default false)
/// </summary>
public class ProductCreateDto
{
    // [Required] = field ini wajib ada di request body
    // Jika tidak ada, ASP.NET Core akan return 400 Bad Request otomatis
    [Required(ErrorMessage = "Nama produk wajib diisi")]
    [StringLength(200, MinimumLength = 3,
        ErrorMessage = "Nama produk minimal 3 karakter, maksimal 200 karakter")]
    public string Name { get; set; } = string.Empty;

    // Nullable: description boleh tidak diisi
    [StringLength(2000, ErrorMessage = "Deskripsi maksimal 2000 karakter")]
    public string? Description { get; set; }

    // [Range] = validasi range nilai
    [Required(ErrorMessage = "Harga wajib diisi")]
    [Range(0, double.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stok wajib diisi")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
    public int Stock { get; set; }
}

// ── UPDATE DTO ────────────────────────────────────────────────
// Digunakan untuk request PUT /api/products/{id}
// PUT = update SELURUH resource
// PATCH = update SEBAGIAN resource (tidak diimplementasi di sini)

/// <summary>
/// DTO untuk mengupdate product yang sudah ada.
/// Sama dengan CreateDto, tapi ID diambil dari URL parameter.
/// </summary>
public class ProductUpdateDto
{
    [Required(ErrorMessage = "Nama produk wajib diisi")]
    [StringLength(200, MinimumLength = 3,
        ErrorMessage = "Nama produk minimal 3 karakter, maksimal 200 karakter")]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000, ErrorMessage = "Deskripsi maksimal 2000 karakter")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Harga wajib diisi")]
    [Range(0, double.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stok wajib diisi")]
    [Range(0, int.MaxValue, ErrorMessage = "Stok tidak boleh negatif")]
    public int Stock { get; set; }
}

// ── RESPONSE DTO ──────────────────────────────────────────────
// Digunakan untuk response GET /api/products dan POST/PUT response
// Berisi field yang AMAN untuk dikirim ke client

/// <summary>
/// DTO untuk response. Berisi semua field yang ditampilkan ke client.
/// Field yang TIDAK ada: IsDeleted (internal), DeletedAt (internal)
/// </summary>
public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // CreatedAt: informasi berguna untuk client (sorting, display)
    public DateTime CreatedAt { get; set; }

    // UpdatedAt: nullable karena mungkin belum pernah diupdate
    public DateTime? UpdatedAt { get; set; }
}

// ── QUERY / FILTER DTO ────────────────────────────────────────
// Digunakan untuk parameter query: GET /api/products?search=...&page=...

/// <summary>
/// Parameter query untuk GET semua products.
/// Semua field nullable karena bersifat opsional.
/// </summary>
public class ProductQueryDto
{
    // Search keyword — mencari di Name dan Description
    public string? Search { get; set; }

    // Sorting
    // SortBy: "name", "price", "stock", "createdAt"
    public string? SortBy { get; set; }

    // SortDesc: true = Z-A atau high-to-low, false = A-Z atau low-to-high
    public bool SortDesc { get; set; } = false;

    // Pagination
    // Page: nomor halaman, minimal 1
    private int _page = 1;
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value; // Auto-correct jika < 1
    }

    // PageSize: jumlah data per halaman, max 100 agar tidak overload
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > 100 ? 100 : value < 1 ? 1 : value; // Clamp 1-100
    }

    // Filter harga (opsional)
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
