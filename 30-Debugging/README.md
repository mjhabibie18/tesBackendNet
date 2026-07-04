# 🔍 30 — Debugging

## Debugging di ASP.NET Core

Debugging adalah skill yang membedakan developer junior dan senior.

---

## Tools Debugging

| Tool | Digunakan Untuk |
|------|----------------|
| **Visual Studio Debugger** | Breakpoint, step-through, watch |
| **VS Code Debugger** | Same, lighter |
| **dotnet-trace** | Performance profiling |
| **dotnet-dump** | Memory dump analysis |
| **Swagger UI** | Test endpoint manual |
| **Postman** | Test HTTP request |
| **SQL Profiler** | Monitor SQL query |
| **Application Insights** | Production monitoring |

---

## Breakpoint Debugging

```csharp
// Set breakpoint di Visual Studio/VS Code
// F9 = toggle breakpoint
// F5 = start debugging
// F10 = step over
// F11 = step into
// Shift+F11 = step out
// F5 = continue

// Conditional Breakpoint: hanya stop jika kondisi terpenuhi
// Right-click breakpoint → Conditions → "id == 5"
```

---

## Logging untuk Debugging

```csharp
// Tambahkan log di titik yang dicurigai
_logger.LogDebug("Data masuk: {@dto}", dto);
_logger.LogDebug("Query result: {Count} items", items.Count);
_logger.LogDebug("Product setelah update: {@product}", product);

// Structured logging: bisa di-filter
_logger.LogInformation("User {UserId} melakukan aksi {Action} pada {Timestamp}",
    userId, action, DateTime.UtcNow);
```

---

## EF Core SQL Logging

```csharp
// Tampilkan SQL yang dieksekusi EF Core
optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
optionsBuilder.EnableSensitiveDataLogging(); // Tampilkan nilai parameter

// Atau via appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

---

## Common Bugs & Solutions

### 1. NullReferenceException
```csharp
// ❌ Tanpa null check
var name = product.Category.Name; // NullReferenceException jika Category null!

// ✅ Null-safe
var name = product.Category?.Name ?? "Uncategorized";
```

### 2. N+1 Problem (EF Core)
```csharp
// Cek di log SQL: terlalu banyak query
// Solusi: eager loading
var products = await _context.Products
    .Include(p => p.Category) // Fix N+1
    .ToListAsync();
```

### 3. Connection Leak
```csharp
// ❌ Connection tidak di-close
var conn = new SqlConnection(connStr);
conn.Open();
// Lupa conn.Close()!

// ✅ Using statement: auto-close
using var conn = new SqlConnection(connStr);
// Atau gunakan EF Core (handle koneksi otomatis)
```

### 4. Async Deadlock
```csharp
// ❌ Deadlock: .Result atau .Wait() dalam async context
var result = _service.GetAllAsync().Result; // DEADLOCK!

// ✅ Async all the way
var result = await _service.GetAllAsync();
```

---

## HTTP Debugging

```bash
# Test endpoint dari terminal
curl -X GET https://localhost:7001/api/products \
  -H "Authorization: Bearer eyJ..." \
  -H "Content-Type: application/json"

# Verbose output
curl -v https://localhost:7001/api/products
```

---

## ✅ Best Practice

1. **Read error message dengan teliti**: sering ada petunjuk di dalamnya
2. **Check logs dulu** sebelum debug
3. **Isolate masalah**: sederhanakan sampai menemukan root cause
4. **Reproduce dulu**: pastikan bisa reproduce sebelum fix
5. **Unit test untuk prevent regression**: tulis test setelah fix bug

---

## 🎤 Tips Interview

**Q: "Bagaimana proses debugging kamu?"**
```
1. Baca error message dengan teliti
2. Cek log aplikasi
3. Reproduce masalah di environment yang sama
4. Isolate: persempit scope masalah
5. Gunakan debugger (breakpoint, watch)
6. Fix dan tulis unit test untuk prevent regression
7. Verifikasi fix tidak break sesuatu yang lain
```
