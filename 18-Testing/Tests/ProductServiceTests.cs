// ============================================================
// ProductServiceTests.cs — Unit Tests Menggunakan xUnit & Moq
// ============================================================
// Unit Testing menguji fungsionalitas unit terkecil (service)
// secara terisolasi penuh dengan mem-mock repository data layer.
// ============================================================

using FluentAssertions;
using Moq;
using TesBackendNet.TestingDemo.DTOs;
using TesBackendNet.TestingDemo.Models;
using TesBackendNet.TestingDemo.Repositories;
using TesBackendNet.TestingDemo.Services;
using Xunit;

namespace TesBackendNet.TestingDemo.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        // ── Inisialisasi Mock Repository & Service ───────────────
        _mockRepo = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepo.Object);
    }

    // 1. Uji scenario Error saat membuat produk duplikat
    [Fact]
    public async Task CreateAsync_WhenNameExists_ThrowsException()
    {
        // Arrange
        var dto = new ProductCreateDto { Name = "Laptop Gaming", Price = 15000000, Stock = 5 };
        _mockRepo.Setup(r => r.ExistsByNameAsync(dto.Name)).ReturnsAsync(true);

        // Act
        var act = async () => await _service.CreateAsync(dto);

        // Assert (Menggunakan FluentAssertions agar lebih readable)
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*sudah ada*");
    }

    // 2. Uji data dengan parameter beruntun (Inline Data)
    [Theory]
    [InlineData(1, "Laptop", 10000000, 5)]
    [InlineData(2, "Mouse Wireless", 150000, 20)]
    public async Task GetByIdAsync_WhenExists_ReturnsDto(
        int id, string name, decimal price, int stock)
    {
        // Arrange
        var product = new Product { Id = id, Name = name, Price = price, Stock = stock };
        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(product);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be(name);
        result.Price.Should().Be(price);
        result.Stock.Should().Be(stock);
    }

    // 3. Uji scenario pencarian data kosong
    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }
}
