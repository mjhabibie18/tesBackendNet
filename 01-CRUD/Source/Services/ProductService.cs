// ============================================================
// ProductService.cs — Business Logic Layer
// ============================================================
// Service bertanggung jawab untuk:
//   1. Validasi bisnis (contoh: cek duplikat nama)
//   2. Mapping Entity ↔ DTO
//   3. Orchestrasi: panggil Repository yang diperlukan
//   4. Throw exception yang bermakna untuk error handling
//
// Mengapa Service terpisah dari Controller?
//   - Reusability: bisa dipakai dari banyak Controller atau Background Job
//   - Testability: unit test cukup test Service, mock Repository
//   - SRP: Controller hanya handle HTTP, Service handle business
// ============================================================

using TesBackendNet.CRUD.Common;
using TesBackendNet.CRUD.DTO;
using TesBackendNet.CRUD.Models;
using TesBackendNet.CRUD.Repositories;

namespace TesBackendNet.CRUD.Services;

/// <summary>
/// Implementasi business logic untuk Product.
/// Menggunakan IProductRepository untuk akses data.
/// </summary>
public class ProductService : IProductService
{
    // ── Dependencies ──────────────────────────────────────────
    // Menggunakan interface IProductRepository, bukan ProductRepository langsung.
    // Ini memastikan Service tidak tergantung pada implementasi spesifik.
    private readonly IProductRepository _repository;

    // Di aplikasi nyata, biasanya tambahkan:
    // private readonly ILogger<ProductService> _logger;
    // private readonly IMapper _mapper;  // jika pakai AutoMapper

    // ── Constructor ───────────────────────────────────────────
    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    // ── GetAll ─────────────────────────────────────────────────
    /// <summary>
    /// Mengambil semua product dengan filter, sort, dan pagination.
    ///
    /// Flow:
    /// 1. Panggil Repository untuk ambil data + total count
    /// 2. Map setiap Product entity ke ProductResponseDto
    /// 3. Bungkus dalam PagedResult dengan metadata pagination
    /// </summary>
    public async Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryDto query)
    {
        // Repository mengembalikan tuple: (data, totalCount)
        // Destructuring tuple: var (data, total) = ...
        var (data, totalCount) = await _repository.GetAllAsync(query);

        // Map List<Product> → List<ProductResponseDto>
        // Select() = LINQ untuk transform setiap element
        // MapToDto() = helper method di bawah untuk mapping
        var dtoList = data.Select(MapToDto).ToList();

        // Bungkus dalam PagedResult
        return new PagedResult<ProductResponseDto>
        {
            Data         = dtoList,
            TotalCount   = totalCount,
            CurrentPage  = query.Page,
            PageSize     = query.PageSize
            // TotalPages, HasNextPage, HasPreviousPage dihitung otomatis di PagedResult
        };
    }

    // ── GetById ────────────────────────────────────────────────
    /// <summary>
    /// Mengambil satu product berdasarkan ID.
    ///
    /// Return null jika tidak ditemukan (Controller yang handle 404).
    /// </summary>
    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        // Jika null (tidak ditemukan), return null
        // Controller akan return 404 Not Found
        if (product == null) return null;

        return MapToDto(product);
    }

    // ── Create ─────────────────────────────────────────────────
    /// <summary>
    /// Membuat product baru.
    ///
    /// Flow:
    /// 1. Validasi bisnis: cek duplikat nama
    /// 2. Map DTO → Entity
    /// 3. Simpan ke database
    /// 4. Map Entity → DTO untuk response
    /// </summary>
    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        // ── Validasi Bisnis ───────────────────────────────────
        // Cek apakah product dengan nama yang sama sudah ada
        // Ini adalah VALIDASI BISNIS, bukan validasi input
        // (validasi input dilakukan di DTO dengan DataAnnotations)
        var exists = await _repository.ExistsByNameAsync(dto.Name);
        if (exists)
        {
            // Throw exception — akan ditangkap oleh GlobalExceptionMiddleware
            // Middleware akan convert exception ini ke HTTP 409 Conflict
            throw new InvalidOperationException($"Product dengan nama '{dto.Name}' sudah ada.");
        }

        // ── Mapping DTO → Entity ──────────────────────────────
        // Manual mapping: DTO fields → Entity fields
        // Alternatif: gunakan AutoMapper (lebih ringkas tapi perlu install package)
        var product = new Product
        {
            Name        = dto.Name.Trim(),        // Trim() untuk hapus whitespace
            Description = dto.Description?.Trim(),
            Price       = dto.Price,
            Stock       = dto.Stock,
            IsDeleted   = false,                   // Default: belum dihapus
            CreatedAt   = DateTime.UtcNow          // Server yang set timestamp, bukan client
        };

        // ── Simpan ke Database ────────────────────────────────
        await _repository.AddAsync(product);
        // Setelah AddAsync, product.Id sudah terisi oleh database (IDENTITY)

        // ── Return Response ───────────────────────────────────
        return MapToDto(product);
    }

    // ── Update ─────────────────────────────────────────────────
    /// <summary>
    /// Mengupdate product yang sudah ada.
    ///
    /// Flow:
    /// 1. Ambil product dari database (validasi: exist?)
    /// 2. Validasi bisnis: cek duplikat nama (kecuali diri sendiri)
    /// 3. Update field-field yang berubah
    /// 4. Simpan ke database
    /// 5. Return updated product sebagai DTO
    /// </summary>
    public async Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto dto)
    {
        // ── Cari Product ──────────────────────────────────────
        var product = await _repository.GetByIdAsync(id);

        // Null check: product tidak ditemukan
        // "?? throw" adalah null coalescing throw (C# 7+)
        if (product == null)
            throw new KeyNotFoundException($"Product dengan ID {id} tidak ditemukan.");

        // ── Validasi Duplikat ─────────────────────────────────
        // Cek apakah ada product LAIN dengan nama yang sama
        // excludeId = id: jangan hitung product ini sendiri sebagai duplikat
        // Contoh: Update "Laptop" → masih "Laptop" = OK (bukan duplikat)
        //         Update "Laptop" → "Mouse" tapi "Mouse" sudah ada = DUPLIKAT
        var duplicateExists = await _repository.ExistsByNameAsync(dto.Name, excludeId: id);
        if (duplicateExists)
            throw new InvalidOperationException($"Product dengan nama '{dto.Name}' sudah ada.");

        // ── Update Fields ─────────────────────────────────────
        // Update field-field yang berubah
        // EF Core track perubahan ini karena entity sudah di-track sejak GetByIdAsync
        product.Name        = dto.Name.Trim();
        product.Description = dto.Description?.Trim();
        product.Price       = dto.Price;
        product.Stock       = dto.Stock;
        product.UpdatedAt   = DateTime.UtcNow; // Update timestamp

        // ── Simpan ────────────────────────────────────────────
        await _repository.UpdateAsync(product);

        return MapToDto(product);
    }

    // ── Delete (Soft) ──────────────────────────────────────────
    /// <summary>
    /// Soft delete product.
    /// Tidak benar-benar menghapus dari database.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        // Validasi: cek apakah product ada
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product dengan ID {id} tidak ditemukan.");

        // Tandai sebagai deleted di database
        await _repository.SoftDeleteAsync(id);
    }

    // ── Helper Method ─────────────────────────────────────────
    // Private method untuk mapping Entity → DTO
    // Disimpan sebagai static method karena tidak butuh state dari class
    // Bisa juga jadi extension method atau AutoMapper Profile
    private static ProductResponseDto MapToDto(Product product) => new()
    {
        Id          = product.Id,
        Name        = product.Name,
        Description = product.Description,
        Price       = product.Price,
        Stock       = product.Stock,
        CreatedAt   = product.CreatedAt,
        UpdatedAt   = product.UpdatedAt
    };
}
