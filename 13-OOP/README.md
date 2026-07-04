# 🎯 13 — OOP (Object-Oriented Programming)

## 4 Pilar OOP

### 1. Encapsulation (Enkapsulasi)

Menyembunyikan detail implementasi, hanya expose apa yang perlu.

```csharp
public class BankAccount
{
    private decimal _balance; // Private: tidak bisa diakses langsung

    public decimal Balance => _balance; // Read-only dari luar

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount harus positif");
        _balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (amount > _balance) throw new InvalidOperationException("Saldo tidak cukup");
        _balance -= amount;
    }
}
```

### 2. Inheritance (Pewarisan)

Class turunan mewarisi property dan method dari parent.

```csharp
public abstract class Animal
{
    public string Name { get; set; }
    public abstract string Speak(); // Harus diimplementasi di turunan
    public void Breathe() => Console.WriteLine("Bernafas...");
}

public class Dog : Animal
{
    public override string Speak() => "Woof!";
}

public class Cat : Animal
{
    public override string Speak() => "Meow!";
}
```

### 3. Polymorphism (Polimorfisme)

Satu interface, banyak implementasi.

```csharp
List<Animal> animals = new() { new Dog(), new Cat() };
foreach (var animal in animals)
    Console.WriteLine(animal.Speak()); // Woof! / Meow!
```

### 4. Abstraction (Abstraksi)

Menyembunyikan detail kompleksitas, expose hanya interface.

```csharp
// Interface = kontrak (pure abstraction)
public interface IPaymentGateway
{
    Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber);
}

// Implementasi tersembunyi dari user
public class StripeGateway : IPaymentGateway { ... }
public class MidtransGateway : IPaymentGateway { ... }
```

---

## SOLID Principles

| Prinsip | Singkatan | Deskripsi |
|---------|-----------|-----------|
| **S** | SRP | Single Responsibility: 1 class = 1 tanggung jawab |
| **O** | OCP | Open/Closed: open for extension, closed for modification |
| **L** | LSP | Liskov Substitution: turunan bisa replace parent |
| **I** | ISP | Interface Segregation: interface kecil & spesifik |
| **D** | DIP | Dependency Inversion: depend on abstraction, not implementation |

---

## SRP (Single Responsibility)

```csharp
// ❌ MELANGGAR SRP: satu class terlalu banyak tanggung jawab
public class UserManager
{
    public void CreateUser() { }      // Business logic
    public void SendEmail() { }        // Email logic
    public void SaveToDatabase() { }  // Database logic
    public void LogActivity() { }     // Logging
}

// ✅ SRP: setiap class punya satu tanggung jawab
public class UserService    { public void CreateUser() { } }
public class EmailService   { public void SendEmail() { } }
public class UserRepository { public void Save() { } }
public class Logger         { public void Log() { } }
```

---

## DIP (Dependency Inversion)

```csharp
// ❌ Tergantung pada implementasi konkrit
public class ProductService
{
    private ProductRepository _repo = new ProductRepository(); // Tight coupling!
}

// ✅ Tergantung pada abstraksi (interface)
public class ProductService
{
    private readonly IProductRepository _repo; // Loose coupling!
    public ProductService(IProductRepository repo) => _repo = repo;
}
```

---

## 🎤 Tips Interview

**Q: "Jelaskan SOLID!"**
```
S: Satu class, satu tanggung jawab
O: Tambah fitur dengan class baru (extend), jangan ubah class lama
L: Turunan bisa dipakai di tempat parent tanpa masalah
I: Buat interface kecil & spesifik, jangan interface "all-in-one"
D: Bergantung pada interface, bukan implementasi langsung
```
