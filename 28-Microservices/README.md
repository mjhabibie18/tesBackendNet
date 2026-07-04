# 🔌 28 — Microservices

## Apa itu Microservices?

Microservices adalah arsitektur di mana aplikasi dibagi menjadi **layanan kecil yang independen**, masing-masing bertanggung jawab untuk satu domain bisnis.

---

## Monolith vs Microservices

```
MONOLITH:
┌───────────────────────────────────┐
│              App                  │
│  Users │ Products │ Orders │ ...  │
│              DB                   │
└───────────────────────────────────┘
+ Simple
+ Easy to develop
- Scale all-or-nothing
- Deploy risk: satu bug = semua down

MICROSERVICES:
┌──────────┐  ┌──────────┐  ┌──────────┐
│  User    │  │ Product  │  │  Order   │
│ Service  │  │ Service  │  │ Service  │
│   DB     │  │   DB     │  │   DB     │
└──────────┘  └──────────┘  └──────────┘
      ↑              ↑             ↑
            API Gateway
+ Scale independently
+ Deploy independently
- Complex (network, service discovery)
- Distributed transactions
```

---

## Komunikasi Antar Service

```csharp
// 1. Synchronous: HTTP (REST/gRPC)
// Service A → HTTP call → Service B
public class OrderService
{
    private readonly HttpClient _httpClient;

    public async Task<ProductDto?> GetProductAsync(int productId)
    {
        // Panggil Product Service via HTTP
        var response = await _httpClient.GetAsync(
            $"http://product-service/api/products/{productId}");

        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ProductDto>();
    }
}

// 2. Asynchronous: Message Queue (RabbitMQ, Kafka)
// Service A publish event → Queue → Service B consume
public async Task CreateOrder(CreateOrderDto dto)
{
    var order = await _orderRepo.CreateAsync(dto);

    // Publish event (tidak perlu tunggu Product Service)
    await _publisher.Publish(new OrderCreatedEvent
    {
        OrderId   = order.Id,
        ProductId = dto.ProductId,
        Quantity  = dto.Quantity
    });
}
```

---

## API Gateway

```
Client
  ↓
API Gateway (nginx / Yarp / Ocelot)
  ├── /api/users     → User Service
  ├── /api/products  → Product Service
  └── /api/orders    → Order Service
```

```bash
# Setup Ocelot API Gateway
dotnet add package Ocelot
```

---

## Service Discovery

```
# Consul / Kubernetes Service
# Service tidak perlu tahu IP service lain, cukup nama service
http://product-service/api/products  ← resolved oleh DNS/Service Discovery
```

---

## ✅ Best Practice

1. **Database per Service**: tidak share database
2. **Event-driven communication**: loosely coupled
3. **Circuit Breaker**: jika service down, fail fast (Polly library)
4. **Distributed tracing**: track request antar service (OpenTelemetry)
5. **Health checks**: setiap service punya `/health` endpoint

---

## 🎤 Tips Interview

**Q: "Kapan gunakan Microservices?"**
```
Gunakan Microservices ketika:
- Tim besar (Conway's Law: structure follows team)
- Scale berbeda per domain (Orders vs. Users)
- Deploy independent tiap service

JANGAN gunakan untuk:
- Tim kecil (< 5 orang)
- Aplikasi sederhana
- MVP/startup (mulai monolith dulu!)
```
