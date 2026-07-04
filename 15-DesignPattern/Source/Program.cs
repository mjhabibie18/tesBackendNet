// ============================================================
// Program.cs — Entry Point Demo Design Patterns
// ============================================================

using TesBackendNet.DesignPattern.Patterns;

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🚀 DEMO SOFTWARE DESIGN PATTERNS");
Console.WriteLine("============================================================\n");

// ── 1. Demo Singleton Pattern ─────────────────────────────────
Console.WriteLine("--- 1. SINGLETON PATTERN ---");
var config1 = DatabaseConfigManager.Instance;
var config2 = DatabaseConfigManager.Instance;

Console.WriteLine($"Apakah instance 1 dan 2 sama? {ReferenceEquals(config1, config2)}"); // Harus True
Console.WriteLine($"Connection String: {config1.ConnectionString}");
Console.WriteLine();

// ── 2. Demo Factory Pattern ───────────────────────────────────
Console.WriteLine("--- 2. FACTORY PATTERN ---");
try
{
    var gopayGateway = PaymentGatewayFactory.Create("gopay");
    gopayGateway.Pay(150000);

    var danaGateway = PaymentGatewayFactory.Create("dana");
    danaGateway.Pay(85000);
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
Console.WriteLine();

// ── 3. Demo Strategy Pattern ──────────────────────────────────
Console.WriteLine("--- 3. STRATEGY PATTERN ---");
decimal packageWeight = 10.5m; // 10.5 kg
var calc = new ShippingCalculator(new RegularShipping());

Console.WriteLine($"Biaya Kirim Reguler: Rp{calc.Calculate(packageWeight):N0}");

calc.SetStrategy(new ExpressShipping());
Console.WriteLine($"Biaya Kirim Express: Rp{calc.Calculate(packageWeight):N0}");

calc.SetStrategy(new CargoShipping());
Console.WriteLine($"Biaya Kirim Kargo: Rp{calc.Calculate(packageWeight):N0}");
Console.WriteLine();

// ── 4. Demo Observer Pattern ──────────────────────────────────
Console.WriteLine("--- 4. OBSERVER PATTERN ---");
var orderService = new OrderService();
var emailNotifier = new EmailNotifier();
var inventoryUpdater = new InventoryUpdater();

// Subscribing observers
emailNotifier.Subscribe(orderService);
inventoryUpdater.Subscribe(orderService);

// Triggereing event order creation
orderService.CreateOrder("TRX-998811", 520000);

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Selesai!");
Console.WriteLine("============================================================");
