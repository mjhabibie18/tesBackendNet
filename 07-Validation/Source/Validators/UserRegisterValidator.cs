// ============================================================
// UserRegisterValidator.cs — FluentValidation untuk Registrasi
// ============================================================
// FluentValidation memisahkan aturan validasi dari model DTO, 
// sehingga aturan validasi lebih mudah dibaca dan di-unit test.
// ============================================================

using FluentValidation;
using TesBackendNet.Validation.DTOs;

namespace TesBackendNet.Validation.Validators;

public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
{
    public UserRegisterValidator()
    {
        // ── Validasi Username ────────────────────────────────
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username tidak boleh kosong")
            .MinimumLength(3).WithMessage("Username minimal harus 3 karakter")
            .MaximumLength(20).WithMessage("Username maksimal 20 karakter")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username hanya boleh huruf, angka, dan underscore");

        // ── Validasi Password ────────────────────────────────
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password tidak boleh kosong")
            .MinimumLength(6).WithMessage("Password minimal harus 6 karakter")
            .Matches("[A-Z]").WithMessage("Password harus mengandung minimal 1 huruf besar")
            .Matches("[a-z]").WithMessage("Password harus mengandung minimal 1 huruf kecil")
            .Matches("[0-9]").WithMessage("Password harus mengandung minimal 1 angka");

        // ── Validasi Konfirmasi Password (Cross-Field) ────────
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Konfirmasi password tidak boleh kosong")
            .Equal(x => x.Password).WithMessage("Konfirmasi password tidak cocok dengan password");

        // ── Validasi Nomor Telepon (Regex) ───────────────────
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Nomor telepon tidak boleh kosong")
            .Matches(@"^\+?[0-9]{10,13}$").WithMessage("Format nomor telepon tidak valid");

        // ── Validasi Umur (Range) ────────────────────────────
        RuleFor(x => x.Age)
            .InclusiveBetween(17, 100).WithMessage("Umur pendaftar harus antara 17 sampai 100 tahun");
    }
}
