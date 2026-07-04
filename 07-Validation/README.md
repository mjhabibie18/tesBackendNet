# ✅ 07 — Validation (Validasi)

## Apa itu Validasi?

Validasi adalah proses memastikan bahwa data yang masuk ke sistem sesuai dengan aturan yang ditetapkan.

---

## Jenis Validasi

| Jenis | Lokasi | Contoh |
|-------|--------|--------|
| **Client-side** | Browser/Mobile | Required field, format email |
| **Server-side** | Backend API | WAJIB! Tidak bisa dihindari |
| **Database** | Constraint | NOT NULL, UNIQUE, CHECK |

> **Backend WAJIB validasi sendiri.** Client-side bisa dibypass!

---

## Pendekatan Validasi di ASP.NET Core

### 1. Data Annotations

```csharp
public class ProductCreateDto
{
    [Required(ErrorMessage = "Nama wajib diisi")]
    [StringLength(200, MinimumLength = 3)]
    public string Name { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Harga tidak boleh negatif")]
    public decimal Price { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [RegularExpression(@"^\d{10,13}$", ErrorMessage = "Nomor HP tidak valid")]
    public string Phone { get; set; }
}
```

### 2. FluentValidation (Recommended)

```bash
dotnet add package FluentValidation.AspNetCore
```

```csharp
public class ProductValidator : AbstractValidator<ProductCreateDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nama wajib diisi")
            .MinimumLength(3).WithMessage("Nama minimal 3 karakter")
            .MaximumLength(200).WithMessage("Nama maksimal 200 karakter");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Harga tidak boleh negatif");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stok tidak boleh negatif");

        // Custom rule
        RuleFor(x => x.Price)
            .Must(price => price % 100 == 0).WithMessage("Harga harus kelipatan 100");
    }
}
```

---

## Response Validasi Error

```json
// Standar ASP.NET Core [ApiController] (400 Bad Request)
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["Nama wajib diisi"],
    "Price": ["Harga tidak boleh negatif"]
  }
}
```

---

## ✅ Best Practice

1. **Validasi di setiap layer**: DTO, Service (business), Database
2. **Pesan error yang jelas**: user tahu apa yang salah
3. **Bahasa yang konsisten**: Indonesian atau English, pilih satu
4. **Jangan expose detail internal**: "Database error" bukan untuk user
5. **Sanitize input**: trim whitespace, normalize case

---

## 🎤 Tips Interview

**Q: "Di mana sebaiknya validasi dilakukan?"**
```
1. DTO level (DataAnnotations/FluentValidation): format, required, range
2. Service level: business rules (cek duplikat, cek relasi)
3. Database level: constraint sebagai safety net terakhir
Semua layer diperlukan!
```
