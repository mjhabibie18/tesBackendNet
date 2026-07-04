// ============================================================
// Program.cs — Entry Point Demo OOP & SOLID
// ============================================================

using TesBackendNet.OOP.Pillars;
using TesBackendNet.OOP.Solid;

Console.Clear();
Console.WriteLine("============================================================");
Console.WriteLine("🚀 DEMO PILAR OOP & SOLID PRINCIPLES");
Console.WriteLine("============================================================\n");

// ── 1. Demo Encapsulation ─────────────────────────────────────
Console.WriteLine("--- 1. ENKAPSULASI (Encapsulation) ---");
var myAccount = new BankAccount();
myAccount.Deposit(1500000);
Console.WriteLine($"Saldo setelah deposit: Rp{myAccount.Balance:N0}");
try
{
    myAccount.Withdraw(2000000); // Memicu error saldo kurang
}
catch (Exception ex)
{
    Console.WriteLine($"[Expected Error]: {ex.Message}");
}
Console.WriteLine();

// ── 2. Demo Inheritance & Polymorphism ────────────────────────
Console.WriteLine("--- 2. PEWARISAN & POLIMORFISME (Inheritance & Polymorphism) ---");
var animals = new List<Animal>
{
    new Dog { Name = "Blacky" },
    new Cat { Name = "Kitty" }
};

foreach (var animal in animals)
{
    Console.WriteLine($"{animal.Name} bersuara: {animal.Speak()}");
    animal.Sleep(); // Sleep adalah virtual method
}
Console.WriteLine();

// ── 3. Demo Abstraction ───────────────────────────────────────
Console.WriteLine("--- 3. ABSTRAKSI (Abstraction) ---");
IPaymentGateway gateway = new MidtransGateway(); // Bisa dengan mudah diganti ke StripeGateway
gateway.ProcessPayment(750000, "4512-xxxx-xxxx-1234");
Console.WriteLine();

// ── 4. Demo SOLID: Single Responsibility Principle (SRP) ──────
Console.WriteLine("--- 4. SOLID: Single Responsibility Principle (SRP) ---");
var db = new DatabaseService();
var email = new EmailNotificationService();
var userService = new UserService(db, email);
userService.CreateUser("mjhabibie18");
Console.WriteLine();

// ── 5. Demo SOLID: Dependency Inversion Principle (DIP) ──────
Console.WriteLine("--- 5. SOLID: Dependency Inversion Principle (DIP) ---");
// Kita bisa menukar implementasi repository tanpa merubah kode GoodProductService sedikit pun!
IProductRepository sqlRepo = new SqlRepository();
var productServiceSql = new GoodProductService(sqlRepo);
productServiceSql.AddProduct("Laptop Gaming ROG");

Console.WriteLine("----------------------------------");

IProductRepository mongoRepo = new MongoRepository();
var productServiceMongo = new GoodProductService(mongoRepo);
productServiceMongo.AddProduct("Smartphone flagship");

Console.WriteLine("\n============================================================");
Console.WriteLine("Demo Selesai!");
Console.WriteLine("============================================================");
