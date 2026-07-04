using TesBackendNet.TestingDemo.DTOs;
using TesBackendNet.TestingDemo.Models;
using TesBackendNet.TestingDemo.Repositories;

namespace TesBackendNet.TestingDemo.Services;

public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product == null) return null;

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }

    public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
    {
        // Validasi keunikan nama
        if (await _repo.ExistsByNameAsync(dto.Name))
        {
            throw new InvalidOperationException($"Produk dengan nama '{dto.Name}' sudah ada.");
        }

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock
        };

        await _repo.AddAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Stock = product.Stock
        };
    }
}
