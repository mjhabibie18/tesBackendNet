# 📬 20 — Queue & Background Jobs

## Mengapa Queue?

Queue memisahkan **long-running tasks** dari HTTP request sehingga user mendapat response cepat.

```
Tanpa Queue:
POST /api/orders → Proses order → Kirim email → Return (5-10 detik!)

Dengan Queue:
POST /api/orders → Return 202 Accepted (< 100ms!)
                 ↓ (background)
                 Process order + Send email
```

---

## Use Case Queue

- ✅ Kirim email
- ✅ Push notification
- ✅ Generate laporan PDF
- ✅ Resize/process gambar
- ✅ Sinkronisasi data ke sistem lain
- ✅ Retry failed operations

---

## 1. Hangfire (Simple, In-Process)

```bash
dotnet add package Hangfire.AspNetCore
dotnet add package Hangfire.SqlServer
```

```csharp
// Program.cs
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

app.UseHangfireDashboard("/hangfire"); // Dashboard: /hangfire

// Enqueue job
[HttpPost("orders")]
public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
{
    var order = await _orderService.CreateAsync(dto);

    // Tambahkan ke queue — diproses oleh Hangfire worker
    BackgroundJob.Enqueue<IEmailService>(
        email => email.SendOrderConfirmationAsync(order.Id));

    return Accepted(new { orderId = order.Id, message = "Order dibuat, email akan dikirim segera" });
}

// Scheduled/Recurring Job
RecurringJob.AddOrUpdate<IReportService>(
    "daily-report",
    service => service.GenerateDailyReportAsync(),
    Cron.Daily);  // Setiap hari jam 00:00
```

---

## 2. RabbitMQ (Distributed Queue)

```bash
dotnet add package RabbitMQ.Client
dotnet add package MassTransit.RabbitMQ
```

```csharp
// Publisher
public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public string CustomerEmail { get; set; }
    public decimal TotalAmount { get; set; }
}

// Publish event
await _publishEndpoint.Publish(new OrderCreatedEvent
{
    OrderId       = order.Id,
    CustomerEmail = order.CustomerEmail,
    TotalAmount   = order.Total
});

// Consumer
public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;
        await _emailService.SendOrderConfirmationAsync(order.CustomerEmail, order.OrderId);
    }
}
```

---

## ✅ Best Practice

1. **Idempotent consumer**: proses message berulang tidak merusak data
2. **Dead letter queue**: tampung message yang gagal diproses
3. **Retry dengan exponential backoff**: retry setelah 1s, 2s, 4s, 8s
4. **Monitor queue**: Hangfire Dashboard atau RabbitMQ Management UI

---

## 🎤 Tips Interview

**Q: "Apa bedanya Hangfire dan RabbitMQ?"**
```
Hangfire: simple, in-process, cocok untuk satu server, persistence di SQL
RabbitMQ: distributed, cocok untuk microservices, high throughput
→ Gunakan Hangfire untuk mulai, migrasi ke RabbitMQ jika perlu scale
```
