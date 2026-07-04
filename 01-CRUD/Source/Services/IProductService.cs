// ============================================================
// IProductService.cs — Interface Service Layer
// ============================================================
// Service adalah lapisan antara Controller dan Repository.
// Interface ini mendefinisikan OPERASI BISNIS yang tersedia.
//
// Bedanya Service vs Repository:
//   Repository: "Bagaimana cara akses database?"
//   Service:    "Bagaimana cara memproses bisnis?"
//
// Service bertugas:
//   - Validasi bisnis (bukan validasi input — itu di DTO/Controller)
//   - Orchestrasi (panggil beberapa repository jika perlu)
//   - Transform data (Entity → DTO dan sebaliknya)
//   - Throw exception yang sesuai untuk error handling
// ============================================================

using TesBackendNet.CRUD.Common;
using TesBackendNet.CRUD.DTO;

namespace TesBackendNet.CRUD.Services;

/// <summary>
/// Kontrak untuk operasi bisnis Product.
/// Controller hanya berinteraksi dengan interface ini, tidak langsung ke Repository.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Mengambil semua product dengan filter, sort, dan pagination.
    /// </summary>
    Task<PagedResult<ProductResponseDto>> GetAllAsync(ProductQueryDto query);

    /// <summary>
    /// Mengambil satu product berdasarkan ID.
    /// Return null jika tidak ditemukan.
    /// </summary>
    Task<ProductResponseDto?> GetByIdAsync(int id);

    /// <summary>
    /// Membuat product baru.
    /// Throw exception jika nama sudah ada.
    /// </summary>
    Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);

    /// <summary>
    /// Mengupdate product yang sudah ada.
    /// Throw exception jika ID tidak ditemukan atau nama duplikat.
    /// </summary>
    Task<ProductResponseDto> UpdateAsync(int id, ProductUpdateDto dto);

    /// <summary>
    /// Soft delete product.
    /// Throw exception jika ID tidak ditemukan.
    /// </summary>
    Task DeleteAsync(int id);
}
