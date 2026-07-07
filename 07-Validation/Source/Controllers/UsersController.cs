// ============================================================
// Nama File: UsersController.cs — Controller Registrasi User dengan FluentValidation
// Folder: 07-Validation/Source/Controllers/
// ============================================================
// 1. PENJELASAN FOLDER (Validation):
//    - Tujuan: Memvalidasi semua input dari klien sebelum diproses ke lapisan bisnis atau database.
//    - Kapan Digunakan: Setiap endpoint yang menerima data dari klien (POST, PUT, PATCH) wajib divalidasi.
//    - Hubungan: Controller ini menggunakan IValidator<T> dari FluentValidation yang dikonfigurasi di Program.cs.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menerima request registrasi, menjalankan validasi FluentValidation secara eksplisit,
//      dan mengembalikan error terstruktur per field jika validasi gagal.
//    - Mengapa Diperlukan: Menunjukkan pola validasi yang dapat dipresentasikan di interview sebagai best practice.
//    - Hubungan File: Menggunakan UserRegisterDto.cs sebagai input dan IValidator dari UserRegisterValidator.cs.
//    - Jika Dihapus: Tidak ada demonstrasi validasi input yang fungsional.
// ============================================================

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TesBackendNet.Validation.DTOs;

namespace TesBackendNet.Validation.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller yang mengelola proses registrasi pengguna dengan validasi input komprehensif.
/// 
/// DUA PENDEKATAN VALIDASI FLUENT:
/// 1. Eksplisit (digunakan di sini): `await _validator.ValidateAsync(dto)` dipanggil manual di controller.
///    Keunggulan: Kontrol penuh atas kapan validasi dijalankan dan bagaimana error ditangani.
/// 2. Otomatis (via filter pipeline): Mendaftarkan `.AddFluentValidationAutoValidation()` di Program.cs.
///    Keunggulan: Validasi berjalan otomatis sebelum action method dipanggil, meminimalkan boilerplate code.
///    Kekurangan: Error handling perlu dikustomisasi melalui filter terpisah.
/// </summary>
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IValidator<UserRegisterDto> _validator;

    /// <summary>
    /// CONSTRUCTOR: Menyuntikkan validator dari DI Container.
    /// IValidator<UserRegisterDto> dikonfigurasi di Program.cs melalui `services.AddValidatorsFromAssembly(...)`.
    /// </summary>
    public UsersController(IValidator<UserRegisterDto> validator)
    {
        _validator = validator;
    }

    /// <summary>
    /// FUNGSI METHOD: Menerima dan memproses request registrasi pengguna baru.
    /// PARAMETER: UserRegisterDto (payload JSON dari klien).
    /// NILAI KEMBALIAN: HTTP 200 OK jika validasi lolos, atau HTTP 400 Bad Request berisi error terstruktur per field.
    /// 
    /// ALUR VALIDASI:
    /// 1. `_validator.ValidateAsync(dto)` menjalankan semua rule di UserRegisterValidator.cs.
    /// 2. Jika ada rule yang dilanggar, errors dikelompokkan per field menggunakan LINQ GroupBy.
    /// 3. Format error terstruktur dikembalikan dalam bentuk Dictionary<string, string[]>.
    /// 
    /// FORMAT ERROR YANG DIKEMBALIKAN:
    /// {
    ///   "Username": ["Username minimal 3 karakter"],
    ///   "Password": ["Password wajib diisi", "Password minimal 6 karakter"]
    /// }
    /// Dengan format ini, frontend (React, Vue, Flutter) dapat menampilkan pesan error
    /// langsung di bawah form field yang bermasalah secara presisi.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        // ── Jalankan Semua Aturan Validasi ─────────────────────────
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            // ── Kelompokkan & Petakan Error Per Field (LINQ) ────────
            // 
            // `validationResult.Errors`: List<ValidationFailure> berisi semua rule yang dilanggar.
            // Setiap ValidationFailure memiliki: PropertyName (nama field), ErrorMessage (pesan error).
            //
            // `.GroupBy(e => e.PropertyName)`: Kelompokkan error berdasarkan nama field.
            //    Satu field bisa memiliki lebih dari satu pesan error jika melanggar banyak rule.
            //    Contoh: "Password" bisa melanggar "MinLength" DAN "Must mengandung angka".
            //
            // `.ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())`:
            //    Konversi ke Dictionary<string, string[]>:
            //    - Key (g.Key)     : Nama field/property yang bermasalah.
            //    - Value (string[]) : Array pesan error untuk field tersebut.
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new
            {
                Success = false,
                Message = "Proses validasi gagal.",
                Errors  = errors
            });
        }

        return Ok(new
        {
            Success = true,
            Message = "Registrasi user berhasil!",
            Data    = new
            {
                dto.Username,
                dto.Email
            }
        });
    }
}
