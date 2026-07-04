# 🏗️ 15 — Design Pattern

## Apa itu Design Pattern?

Design Pattern adalah **solusi umum untuk masalah umum** dalam pengembangan software. Bukan library atau framework, tapi template solusi.

---

## Kategori Design Pattern

| Kategori | Deskripsi | Contoh |
|----------|-----------|--------|
| **Creational** | Cara membuat object | Singleton, Factory, Builder |
| **Structural** | Cara menyusun class | Repository, Decorator, Facade |
| **Behavioral** | Cara komunikasi antar class | Observer, Strategy, Command |

---

## 1. Repository Pattern

```csharp
// Sudah dicover di modul CRUD!
// Interface memisahkan data access dari business logic
public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task AddAsync(Product product);
}
```

---

## 2. Singleton Pattern

```csharp
// Hanya satu instance di seluruh aplikasi
// Di ASP.NET Core: AddSingleton<T>()
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();

// Implementasi manual (tidak disarankan di ASP.NET Core, gunakan DI)
public sealed class ConfigManager
{
    private static ConfigManager? _instance;
    private static readonly object _lock = new();

    private ConfigManager() { }

    public static ConfigManager Instance
    {
        get
        {
            lock (_lock)
                return _instance ??= new ConfigManager();
        }
    }
}
```

---

## 3. Factory Pattern

```csharp
public interface IPaymentProcessor
{
    Task<bool> ProcessAsync(decimal amount);
}

public class PaymentProcessorFactory
{
    public static IPaymentProcessor Create(string method) => method switch
    {
        "stripe"   => new StripeProcessor(),
        "paypal"   => new PayPalProcessor(),
        "midtrans" => new MidtransProcessor(),
        _ => throw new ArgumentException($"Payment method '{method}' tidak dikenal")
    };
}

// Penggunaan
var processor = PaymentProcessorFactory.Create("stripe");
await processor.ProcessAsync(100_000);
```

---

## 4. Observer Pattern

```csharp
// Di .NET: gunakan event dan EventHandler
public class OrderService
{
    // Event yang bisa disubscribe oleh listener lain
    public event EventHandler<Order>? OrderCreated;

    public async Task<Order> CreateAsync(CreateOrderDto dto)
    {
        var order = new Order { ... };
        await _repo.AddAsync(order);

        // Trigger event — semua subscriber akan dipanggil
        OrderCreated?.Invoke(this, order);
        return order;
    }
}

public class EmailNotificationService
{
    public EmailNotificationService(OrderService orderService)
    {
        // Subscribe ke event
        orderService.OrderCreated += async (sender, order) =>
            await SendOrderEmailAsync(order);
    }
}
```

---

## 5. Strategy Pattern

```csharp
public interface ISortStrategy<T>
{
    IEnumerable<T> Sort(IEnumerable<T> items);
}

public class PriceSortStrategy : ISortStrategy<Product>
{
    public IEnumerable<Product> Sort(IEnumerable<Product> items)
        => items.OrderBy(p => p.Price);
}

public class NameSortStrategy : ISortStrategy<Product>
{
    public IEnumerable<Product> Sort(IEnumerable<Product> items)
        => items.OrderBy(p => p.Name);
}

// Context
public class ProductSorter
{
    private ISortStrategy<Product> _strategy;

    public ProductSorter(ISortStrategy<Product> strategy) => _strategy = strategy;
    public void SetStrategy(ISortStrategy<Product> strategy) => _strategy = strategy;
    public IEnumerable<Product> Sort(IEnumerable<Product> items) => _strategy.Sort(items);
}
```

---

## 🎤 Tips Interview

**Q: "Design Pattern apa yang sering kamu gunakan?"**
```
Repository Pattern: setiap project backend
Singleton: cache, config manager (via DI)
Factory: payment gateway, notification channel
Strategy: sorting, pricing algorithm
Observer: event-driven (email setelah order)
```
