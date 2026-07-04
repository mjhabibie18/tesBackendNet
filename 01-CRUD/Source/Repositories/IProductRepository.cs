// ============================================================
// IProductRepository.cs — Interface Repository
// ============================================================
// Interface mendefinisikan KONTRAK — apa yang bisa dilakukan,
// tanpa peduli BAGAIMANA melakukannya.
//
// Mengapa menggunakan Interface?
//   1. Dependency Inversion Principle (DIP):
//      Service bergantung pada abstraksi (interface), bukan implementasi
//   2. Unit Testing:
//      Bisa mock/stub interface di unit test tanpa koneksi database
//   3. Flexibility:
//      Bisa swap implementasi (EF Core → Dapper → dll) tanpa ubah Service
//   4. Kontrak yang jelas:
//      Developer lain tahu method apa yang tersedia
//
// Alur DI:
//   builder.Services.AddScoped<IProductRepository, ProductRepository>()
//   → Saat ProductService butuh IProductRepository,
//     DI Container inject ProductRepository
// ============================================================

using TesBackendNet.CRUD.DTO;
using TesBackendNet.CRUD.Models;

namespace TesBackendNet.CRUD.Repositories;

/// <summary>
/// Kontrak untuk mengakses data Product di database.
/// Setiap method adalah operasi database yang tersedia.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Mengambil semua product dengan filter, sort, dan pagination.
    /// Return tuple: (data, total count)
    /// </summary>
    Task<(List<Product> Data, int TotalCount)> GetAllAsync(ProductQueryDto query);

    /// <summary>
    /// Mengambil satu product berdasarkan ID.
    /// Return null jika tidak ditemukan (atau sudah soft deleted).
    /// </summary>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// Cek apakah product dengan nama tertentu sudah ada.
    /// excludeId: untuk mengecualikan product saat update (cek duplikat sendiri).
    /// </summary>
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

    /// <summary>Menambahkan product baru ke database.</summary>
    Task AddAsync(Product product);

    /// <summary>Mengupdate product yang sudah ada.</summary>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Soft delete: tandai product sebagai dihapus (IsDeleted = true).
    /// Data tetap ada di database, tidak benar-benar dihapus.
    /// </summary>
    Task SoftDeleteAsync(int id);
}
