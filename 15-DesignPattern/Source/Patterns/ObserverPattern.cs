// ============================================================
// ObserverPattern.cs — Implementasi Observer Pattern
// ============================================================
// Menggunakan event-driven architecture untuk memberi notifikasi
// kepada banyak subscriber ketika terjadi perubahan status.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

public class Order
{
    public string OrderId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}

public class OrderService
{
    // Event yang merepresentasikan perubahan status order (Order Created)
    public event EventHandler<Order>? OnOrderCreated;

    public void CreateOrder(string orderId, decimal total)
    {
        Console.WriteLine($"[OrderService] Membuat Order #{orderId}...");
        
        var order = new Order { OrderId = orderId, TotalAmount = total };

        // Memicu notifikasi ke semua subscriber secara broadcast
        OnOrderCreated?.Invoke(this, order);
    }
}

// ── Subscriber 1: Notification System ──────────────────────
public class EmailNotifier
{
    public void Subscribe(OrderService orderService)
    {
        orderService.OnOrderCreated += SendEmailNotification;
    }

    private void SendEmailNotification(object? sender, Order order)
    {
        Console.WriteLine($"[Email Notifier] Mengirim email struk pembayaran untuk Order #{order.OrderId}");
    }
}

// ── Subscriber 2: Inventory System ─────────────────────────
public class InventoryUpdater
{
    public void Subscribe(OrderService orderService)
    {
        orderService.OnOrderCreated += UpdateStock;
    }

    private void UpdateStock(object? sender, Order order)
    {
        Console.WriteLine($"[Inventory] Mengurangi stok produk untuk Order #{order.OrderId}");
    }
}
