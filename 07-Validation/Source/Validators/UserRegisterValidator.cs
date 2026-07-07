// ============================================================
// Nama File: UserRegisterValidator.cs — FluentValidation Rules untuk Registrasi User
// Folder: 07-Validation/Source/Validators/
// ============================================================
// 1. PENJELASAN FOLDER (Validation/Validators):
//    - Tujuan: Memusatkan semua aturan validasi input di satu lokasi, terpisah dari DTO dan Controller.
//    - Kapan Digunakan: Setiap kali ada request masuk yang perlu divalidasi sebelum diproses.
//    - Hubungan: Kelas ini diinjeksikan ke UsersController.cs via IValidator<UserRegisterDto>.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendefinisikan semua aturan validasi untuk UserRegisterDto menggunakan FluentValidation.
//    - Mengapa Diperlukan: FluentValidation lebih ekspresif, testable, dan powerful dibandingkan Data Annotations.
//      Aturan dapat ditulis sebagai metode terpisah, dikombinasikan secara fleksibel, dan diuji dalam unit test.
//    - Hubungan File: Bergantung pada UserRegisterDto.cs sebagai model yang divalidasi.
//    - Jika Dihapus: Tidak ada aturan validasi yang berjalan dan semua input diterima tanpa pemeriksaan.
// ============================================================

using FluentValidation;
using TesBackendNet.Validation.DTOs;

namespace TesBackendNet.Validation.Validators;

/// <summary>
/// TUJUAN CLASS:
/// Kelas validator yang mendefinisikan aturan validasi untuk UserRegisterDto menggunakan FluentValidation.
/// 
/// MENGAPA FLUENT VALIDATION LEBIH BAIK DARI DATA ANNOTATIONS:
/// 1. Pemisahan Tanggung Jawab (SRP): Aturan validasi tidak mencemari kelas DTO/Model.
/// 2. Testability: Validator dapat diuji secara unit test independen tanpa harus menjalankan controller.
/// 3. Fleksibilitas: Mendukung validasi kondisional (When/Unless), async validation (database lookup), 
///    dan cross-field validation (Must, Equal) yang tidak dimungkinkan oleh Data Annotations.
/// 4. Pesan Error Custom: Setiap rule memiliki pesan error yang spesifik dan dapat dilokalisasi.
/// 
/// CARA KERJA:
/// Constructor membangun rantai aturan menggunakan method chaining.
/// Setiap `RuleFor(x => x.PropertyName)` diikuti satu atau lebih aturan validasi.
/// </summary>
public class UserRegisterValidator : AbstractValidator<UserRegisterDto>
{
    /// <summary>
    /// CONSTRUCTOR: Mendefinisikan semua aturan validasi dalam inisialisasi validator.
    /// Aturan dibangun menggunakan Fluent API (method chaining).
    /// </summary>
    public UserRegisterValidator()
    {
        // ── Validasi Username ────────────────────────────────────
        // `.NotEmpty()`: Field tidak boleh null, string kosong "", atau hanya whitespace.
        // `.MinimumLength(3)`: Panjang minimal 3 karakter.
        // `.MaximumLength(20)`: Panjang maksimal 20 karakter.
        // `.Matches(@"^[a-zA-Z0-9_]+$")`: Regex — hanya boleh berisi huruf, angka, atau underscore.
        //   ^ = awal string, $ = akhir string, + = satu karakter atau lebih.
        RuleFor(x => x.Username)
            .NotEmpty()          .WithMessage("Username tidak boleh kosong")
            .MinimumLength(3)    .WithMessage("Username minimal harus 3 karakter")
            .MaximumLength(20)   .WithMessage("Username maksimal 20 karakter")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username hanya boleh huruf, angka, dan underscore");

        // ── Validasi Password (Multi-Rule per Field) ─────────────
        // Setiap `.Matches(...)` adalah regex untuk memastikan password memenuhi kebijakan keamanan.
        // `[A-Z]`: Setidaknya satu huruf kapital.
        // `[a-z]`: Setidaknya satu huruf kecil.
        // `[0-9]`: Setidaknya satu angka.
        // Semua rule dijalankan secara independen — satu field dapat memiliki banyak pesan error sekaligus.
        RuleFor(x => x.Password)
            .NotEmpty()          .WithMessage("Password tidak boleh kosong")
            .MinimumLength(6)    .WithMessage("Password minimal harus 6 karakter")
            .Matches("[A-Z]")    .WithMessage("Password harus mengandung minimal 1 huruf besar")
            .Matches("[a-z]")    .WithMessage("Password harus mengandung minimal 1 huruf kecil")
            .Matches("[0-9]")    .WithMessage("Password harus mengandung minimal 1 angka");

        // ── Validasi Cross-Field: Konfirmasi Password ────────────
        // `.Equal(x => x.Password)`: Membandingkan dua field yang berbeda dalam objek yang sama.
        // Ini adalah Cross-Field Validation — salah satu keunggulan FluentValidation yang tidak dapat dilakukan Data Annotations.
        // Jika Password = "Hello123" dan ConfirmPassword = "Hello456" → rule ini gagal.
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()           .WithMessage("Konfirmasi password tidak boleh kosong")
            .Equal(x => x.Password).WithMessage("Konfirmasi password tidak cocok dengan password");

        // ── Validasi Nomor Telepon (Regex Pattern) ───────────────
        // Regex `^\+?[0-9]{10,13}$`:
        //   `^` = Awal string.
        //   `\+?` = Karakter '+' opsional (untuk kode negara: +62, +1, dll.).
        //   `[0-9]{10,13}` = Tepat 10 hingga 13 digit angka.
        //   `$` = Akhir string.
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()                          .WithMessage("Nomor telepon tidak boleh kosong")
            .Matches(@"^\+?[0-9]{10,13}$")      .WithMessage("Format nomor telepon tidak valid");

        // ── Validasi Umur (Range Inklusif) ────────────────────────
        // `.InclusiveBetween(min, max)`: Nilai harus berada di antara min dan max (termasuk nilai batas).
        // Setara dengan: Age >= 17 && Age <= 100.
        RuleFor(x => x.Age)
            .InclusiveBetween(17, 100).WithMessage("Umur pendaftar harus antara 17 sampai 100 tahun");
    }
}
