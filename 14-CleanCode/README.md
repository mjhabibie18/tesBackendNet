# ✨ 14 — Clean Code

## Prinsip Clean Code

Clean Code adalah kode yang mudah dibaca, dipahami, dan dimaintain oleh developer lain (atau diri sendiri 6 bulan kemudian).

> *"Clean code reads like well-written prose."* — Robert C. Martin (Uncle Bob)

---

## Naming Conventions

```csharp
// ✅ Nama yang bermakna
public class UserAccountService { }          // PascalCase untuk class
private readonly IUserRepository _repo;       // camelCase dengan _ untuk field
public async Task<UserDto> GetByIdAsync(int userId) { } // Method: PascalCase

// ❌ Nama yang tidak bermakna
public class UAS { }
private readonly IUserRepository r;
public async Task<UserDto> Get(int id2) { }

// ✅ Boolean: prefix is/has/can/should
public bool IsActive { get; set; }
public bool HasPermission(string permission) { }
public bool CanDelete(int userId) { }

// ✅ Constants: PascalCase atau ALL_CAPS
public const int MaxRetryCount = 3;
public const string DefaultRole = "User";
```

---

## Functions / Methods

```csharp
// ✅ Satu method = satu tugas (SRP)
// ✅ Nama method menjelaskan apa yang dilakukan
// ✅ Parameter sesedikit mungkin (max 3)

// ❌ Method terlalu panjang dan kompleks
public async Task ProcessUserOrderAndSendEmailAndUpdateInventory(
    int userId, int productId, int quantity, string email,
    string address, string paymentMethod, bool sendSms)
{ /* 200 baris kode */ }

// ✅ Dipecah menjadi beberapa method yang fokus
public async Task CreateOrderAsync(int userId, CreateOrderDto dto)
{
    var order   = await _orderService.CreateAsync(userId, dto);
    await _inventoryService.DecreaseStockAsync(dto.ProductId, dto.Quantity);
    await _emailService.SendOrderConfirmationAsync(userId, order.Id);
}
```

---

## Avoid Magic Numbers/Strings

```csharp
// ❌ Magic numbers
if (user.Role == "1") { }
if (password.Length < 8) { }
await Task.Delay(30000);

// ✅ Named constants
public const string AdminRole = "Admin";
public const int MinPasswordLength = 8;
public const int SessionTimeoutMs = 30_000;

if (user.Role == AdminRole) { }
if (password.Length < MinPasswordLength) { }
await Task.Delay(SessionTimeoutMs);
```

---

## Comments

```csharp
// ❌ Komentar yang menjelaskan "APA" (sudah jelas dari kode)
// Increment i by 1
i++;

// ✅ Komentar yang menjelaskan "MENGAPA"
// Delay 100ms untuk menghindari rate limit dari API eksternal
await Task.Delay(100);

// ✅ XML documentation untuk public API
/// <summary>
/// Mencari user berdasarkan email.
/// Return null jika tidak ditemukan (bukan throw exception).
/// </summary>
public async Task<User?> FindByEmailAsync(string email) { ... }
```

---

## DRY (Don't Repeat Yourself)

```csharp
// ❌ DRY violation
public async Task<IActionResult> CreateProduct([FromBody] ProductDto dto)
{
    if (dto == null) return BadRequest("DTO tidak boleh null");
    if (string.IsNullOrEmpty(dto.Name)) return BadRequest("Nama wajib diisi");
    // ... logic
}

public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto dto)
{
    if (dto == null) return BadRequest("DTO tidak boleh null");          // DUPLIKAT!
    if (string.IsNullOrEmpty(dto.Name)) return BadRequest("Nama wajib diisi"); // DUPLIKAT!
    // ... logic
}

// ✅ Extract ke method/validator
private BadRequestObjectResult? ValidateProductDto(ProductDto dto)
{
    if (dto == null) return BadRequest("DTO tidak boleh null");
    if (string.IsNullOrEmpty(dto.Name)) return BadRequest("Nama wajib diisi");
    return null;
}
```

---

## 🎤 Tips Interview

**Q: "Apa itu Clean Code?"**
```
Kode yang:
1. Readable: mudah dibaca seperti cerita
2. Maintainable: mudah diubah tanpa takut break sesuatu
3. Testable: mudah ditulis unit test
4. DRY: tidak ada duplikasi kode
5. SOLID: mengikuti prinsip SOLID
```
