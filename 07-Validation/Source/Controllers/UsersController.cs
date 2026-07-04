// ============================================================
// UsersController.cs — Controller Registrasi dengan Validasi
// ============================================================

using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TesBackendNet.Validation.DTOs;

namespace TesBackendNet.Validation.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IValidator<UserRegisterDto> _validator;

    public UsersController(IValidator<UserRegisterDto> validator)
    {
        _validator = validator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        // ── Jalankan Validasi Secara Eksplisit ─────────────────
        // (Bisa juga secara otomatis lewat ASP.NET Core filter pipeline)
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            // ── Pemetaan Validation Errors Menggunakan LINQ ──────────────────
            // 1. validationResult.Errors: Merupakan List<ValidationFailure> yang
            //    dihasilkan oleh FluentValidation jika ada aturan yang dilanggar.
            // 
            // 2. GroupBy(e => e.PropertyName): Mengelompokkan objek error berdasarkan
            //    nama properti/field yang bermasalah (contoh: Username, Password).
            //    Ini penting karena satu field bisa melanggar lebih dari satu aturan
            //    (misalnya: Password kosong DAN kurang dari 6 karakter).
            //
            // 3. ToDictionary(...): Mengubah hasil pengelompokan (grouping) menjadi
            //    Struktur Data Dictionary<string, string[]> (Key-Value) di mana:
            //      - Key (g.Key): Nama properti yang bermasalah (misalnya "Password").
            //      - Value (g.Select(...).ToArray()): Mengambil pesan error (ErrorMessage)
            //        dari semua kegagalan di grup tersebut dan mengubahnya menjadi Array String.
            //
            // Hasil akhir variabel 'errors' akan memiliki format terstruktur seperti:
            // {
            //    "Username": ["Username minimal harus 3 karakter"],
            //    "Password": ["Password tidak boleh kosong", "Password harus mengandung minimal 1 angka"]
            // }
            //
            // Pendekatan ini adalah best practice industri agar aplikasi Client (seperti React/Flutter)
            // bisa dengan mudah memetakan dan menampilkan pesan error tepat di bawah form field masing-masing.
            var errors = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            // Mengembalikan HTTP 400 Bad Request bersama payload terstruktur berisi detail error
            return BadRequest(new
            {
                Success = false,
                Message = "Proses validasi gagal.",
                Errors = errors
            });
        }

        return Ok(new
        {
            Success = true,
            Message = "Registrasi user berhasil!",
            Data = new
            {
                dto.Username,
                dto.Email
            }
        });
    }
}
