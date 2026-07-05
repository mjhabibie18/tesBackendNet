// ============================================================
// Nama File: ObserverPattern.cs — Implementasi Observer Pattern (Event-Driven)
// Folder: 15-DesignPattern/Source/Patterns/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendemonstrasikan Observer Pattern menggunakan event C# bawaan (delegate & EventHandler).
//    - Mengapa Diperlukan: Saat satu kejadian (event) perlu memberitahu banyak sistem berbeda (email, stok, log) tanpa
//      ketergantungan langsung antar sistem (loose coupling).
//    - Hubungan File: OrderService adalah Publisher/Subject. EmailNotifier dan InventoryUpdater adalah Subscriber/Observer.
//    - Jika Dihapus: Tidak ada demonstrasi event-driven communication pada modul Design Pattern.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

/// <summary>
/// TUJUAN CLASS:
/// Data model sederhana yang mewakili sebuah pesanan (Order).
/// Digunakan sebagai payload data yang dikirimkan ke semua subscriber saat event terjadi.
/// </summary>
public class Order
{
    /// <summary>ID unik pesanan (misal: "ORD-20240101-001").</summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>Total nilai transaksi pesanan dalam Rupiah.</summary>
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// TUJUAN CLASS:
/// Publisher/Subject dalam Observer Pattern. Memegang daftar subscriber (observer) 
/// dan memberitahu mereka saat order baru berhasil dibuat.
/// 
/// CARA KERJA C# EVENT:
/// - `event EventHandler<Order>? OnOrderCreated` — deklarasi event yang dapat disubscribe.
/// - `EventHandler<TEventArgs>` adalah delegate bawaan .NET untuk event pattern standar.
/// - `OnOrderCreated?.Invoke(this, order)` — memicu event; memanggil semua handler yang terdaftar.
/// - Operator `?.` mencegah NullReferenceException jika belum ada subscriber yang mendaftar.
/// 
/// KEUNTUNGAN DIBANDING MEMANGGIL LANGSUNG:
/// Tanpa Observer Pattern, OrderService harus mengetahui dan memanggil EmailNotifier, InventoryUpdater, dll. secara eksplisit.
/// Ini menciptakan tight coupling. Dengan event, OrderService tidak perlu tahu siapa yang mendengarkan.
/// </summary>
public class OrderService
{
    // Event yang akan di-broadcast ke semua subscriber saat order baru dibuat
    public event EventHandler<Order>? OnOrderCreated;

    /// <summary>
    /// FUNGSI METHOD: Membuat pesanan baru dan memberitahu semua subscriber.
    /// PARAMETER: orderId (ID pesanan), total (nilai transaksi).
    /// 
    /// ALUR EKSEKUSI:
    /// 1. Membuat objek Order baru.
    /// 2. Memicu event OnOrderCreated — semua subscriber dinotifikasi secara sinkron berurutan.
    /// 3. Setiap subscriber menjalankan handler-nya masing-masing secara independen.
    /// </summary>
    public void CreateOrder(string orderId, decimal total)
    {
        Console.WriteLine($"[OrderService] Membuat Order #{orderId}...");
        
        var order = new Order { OrderId = orderId, TotalAmount = total };

        // Memicu notifikasi broadcast ke semua subscriber yang terdaftar
        OnOrderCreated?.Invoke(this, order);
    }
}

// ── Subscriber 1: Email Notification System ────────────────
/// <summary>
/// TUJUAN CLASS:
/// Subscriber pertama yang mengirimkan notifikasi email kepada pelanggan saat order dibuat.
/// 
/// CARA SUBSCRIBE:
/// `orderService.OnOrderCreated += SendEmailNotification` — menambahkan method handler ke event.
/// Method `SendEmailNotification` akan dipanggil secara otomatis setiap kali event OnOrderCreated terjadi.
/// </summary>
public class EmailNotifier
{
    /// <summary>
    /// FUNGSI METHOD: Mendaftarkan diri sebagai subscriber ke event OnOrderCreated.
    /// </summary>
    public void Subscribe(OrderService orderService)
    {
        orderService.OnOrderCreated += SendEmailNotification;
    }

    private void SendEmailNotification(object? sender, Order order)
    {
        Console.WriteLine($"[Email Notifier] Mengirim email struk pembayaran untuk Order #{order.OrderId}");
    }
}

// ── Subscriber 2: Inventory Management System ──────────────
/// <summary>
/// TUJUAN CLASS:
/// Subscriber kedua yang mengurangi stok produk di warehouse saat order baru dibuat.
/// 
/// PRINSIP OPEN/CLOSED:
/// Jika sistem baru perlu ditambahkan (misal: LoyaltyPointsService), kita cukup membuat class baru
/// dan subscribe ke event yang sama. Tidak perlu mengubah OrderService sama sekali.
/// </summary>
public class InventoryUpdater
{
    /// <summary>
    /// FUNGSI METHOD: Mendaftarkan diri sebagai subscriber ke event OnOrderCreated.
    /// </summary>
    public void Subscribe(OrderService orderService)
    {
        orderService.OnOrderCreated += UpdateStock;
    }

    private void UpdateStock(object? sender, Order order)
    {
        Console.WriteLine($"[Inventory] Mengurangi stok produk untuk Order #{order.OrderId}");
    }
}
