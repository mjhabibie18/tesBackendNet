using TesBackendNet.TestingDemo.Models;

namespace TesBackendNet.TestingDemo.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
    Task AddAsync(Product product);
}
