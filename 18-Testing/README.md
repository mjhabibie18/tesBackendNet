# 🧪 18 — Testing (Unit & Integration Test)

## Jenis Testing

| Jenis | Scope | Kecepatan | Isolasi |
|-------|-------|-----------|---------|
| **Unit Test** | Satu class/method | ⚡ Sangat cepat | Penuh (mock dependencies) |
| **Integration Test** | Beberapa layer | 🐢 Lebih lambat | Partial (database nyata) |
| **End-to-End Test** | Seluruh sistem | 🐌 Paling lambat | Tidak ada |

---

## Setup xUnit + Moq

```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package FluentAssertions  # Opsional: assertion yang lebih readable
dotnet add package Microsoft.AspNetCore.Mvc.Testing  # Integration test
```

---

## Unit Test

```csharp
public class ProductServiceTests
{
    // Mock: objek palsu yang simulasi interface
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service  = new ProductService(_mockRepo.Object);
    }

    // [Fact]: satu test case
    [Fact]
    public async Task CreateAsync_WhenNameExists_ThrowsException()
    {
        // Arrange: setup data
        var dto = new ProductCreateDto { Name = "Laptop", Price = 10000, Stock = 5 };
        _mockRepo.Setup(r => r.ExistsByNameAsync(dto.Name, null)).ReturnsAsync(true);

        // Act: jalankan kode yang dites
        var act = async () => await _service.CreateAsync(dto);

        // Assert: verifikasi hasil
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*sudah ada*");
    }

    // [Theory] + [InlineData]: multiple test cases
    [Theory]
    [InlineData(1, "Laptop", 10000, 5)]
    [InlineData(2, "Mouse", 150000, 10)]
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
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(999);

        result.Should().BeNull();
    }
}
```

---

## Integration Test

```csharp
public class ProductControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("success");
    }
}
```

---

## ✅ Best Practice

1. **Naming**: `MethodName_Scenario_ExpectedResult`
2. **AAA Pattern**: Arrange, Act, Assert
3. **One assertion per test** (idealnya)
4. **Test edge cases**: null, empty, boundary
5. **Mock external dependencies**: database, API, email
6. **Test coverage minimal 80%** untuk production

---

## 🎤 Tips Interview

**Q: "Apa bedanya Mock, Stub, dan Fake?"**
```
Mock: object palsu yang bisa verify apakah method dipanggil
Stub: hardcode return value tertentu
Fake: implementasi nyata tapi lebih sederhana (InMemory database)
```
