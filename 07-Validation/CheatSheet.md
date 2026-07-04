# Lembar Contekan (Cheat Sheet) — FluentValidation

Berikut adalah aturan-aturan (rules) validasi dasar yang paling sering digunakan dalam project ASP.NET Core.

---

## 1. Aturan Validasi Standar
```csharp
RuleFor(x => x.Email)
    .NotEmpty().WithMessage("Email wajib diisi")
    .EmailAddress().WithMessage("Format email salah");

RuleFor(x => x.Age)
    .GreaterThanOrEqualTo(18).WithMessage("Minimal berumur 18 tahun");
```

---

## 2. Validasi Custom & Kondisional (Dependent Rules)
```csharp
// Validasi berdasarkan properti lain (Kondisional)
RuleFor(x => x.ConfirmPassword)
    .Equal(x => x.Password).WithMessage("Password konfirmasi harus sama");

// Menjalankan aturan HANYA JIKA kondisi tertentu terpenuhi
RuleFor(x => x.CardNumber)
    .NotEmpty().When(x => x.PaymentMethod == "CreditCard");
```

---

## 3. Registrasi Validator (Program.cs)
```csharp
using FluentValidation;

// Register otomatis semua validator di assembly saat ini
builder.Services.AddValidatorsFromAssemblyContaining<AnyValidatorClass>();
```
