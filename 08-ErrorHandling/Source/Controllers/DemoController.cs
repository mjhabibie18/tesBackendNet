// ============================================================
// Nama File: DemoController.cs — Controller Demo Pemicu Exception
// Folder: 08-ErrorHandling/Source/Controllers/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mengekspos endpoint-endpoint yang sengaja melempar berbagai jenis exception
//      untuk menguji fungsionalitas GlobalExceptionMiddleware.
//    - Mengapa Diperlukan: Membantu developer memahami bahwa exception yang dilempar dari Controller/Service
//      tidak perlu dibungkus dalam blok try-catch — middleware yang bertugas menangkapnya secara otomatis.
//    - Hubungan File: Melempar exception dari AppExceptions.cs yang akan ditangkap di GlobalExceptionMiddleware.cs.
//    - Jika Dihapus: Tidak ada cara untuk menguji apakah GlobalExceptionMiddleware bekerja dengan benar.
// ============================================================

using Microsoft.AspNetCore.Mvc;
using TesBackendNet.ErrorHandling.Exceptions;

namespace TesBackendNet.ErrorHandling.Controllers;

/// <summary>
/// TUJUAN CLASS:
/// Controller demo yang menyediakan berbagai endpoint untuk mensimulasikan skenario error dalam aplikasi backend nyata.
/// 
/// CARA KERJA (Alur Error Handling):
/// Controller tidak memiliki try-catch sama sekali. Setiap exception yang dilempar 
/// akan "naik" (bubble up) melewati middleware pipeline sampai ditangkap oleh GlobalExceptionMiddleware.
/// Ini adalah penerapan prinsip Separation of Concerns (SoC): Controller hanya bertanggung jawab 
/// memicu kondisi error bisnis, bukan menanganinya secara langsung.
/// </summary>
[ApiController]
[Route("api/demo")]
public class DemoController : ControllerBase
{
    /// <summary>
    /// FUNGSI METHOD: Endpoint yang berjalan normal tanpa error.
    /// KEGUNAAN: Sebagai perbandingan (baseline) dengan endpoint yang bermasalah.
    /// HTTP STATUS: 200 OK.
    /// </summary>
    [HttpGet("ok")]
    public IActionResult GetOk() => Ok(new { Message = "Berhasil mengakses endpoint normal!" });

    /// <summary>
    /// FUNGSI METHOD: Mensimulasikan error validasi input dari bisnis (HTTP 400 Bad Request).
    /// 
    /// KAPAN INI TERJADI DI DUNIA NYATA:
    /// Misalnya saat user mengirim tanggal lahir di masa mendatang, atau harga produk bernilai negatif.
    /// Kondisi ini lolos validasi format (JSON valid) tetapi melanggar aturan domain bisnis.
    /// </summary>
    [HttpGet("bad-request")]
    public IActionResult GetBadRequest()
    {
        // Melempar BadRequestException yang akan ditangkap middleware dan dikembalikan sebagai HTTP 400
        throw new BadRequestException("Data input tidak valid atau tidak lengkap.");
    }

    /// <summary>
    /// FUNGSI METHOD: Mensimulasikan resource yang tidak ditemukan (HTTP 404 Not Found).
    /// 
    /// KAPAN INI TERJADI DI DUNIA NYATA:
    /// Misalnya GET /api/products/999 dimana ID 999 tidak ada di database.
    /// </summary>
    [HttpGet("not-found")]
    public IActionResult GetNotFound()
    {
        // Melempar NotFoundException yang akan ditangkap middleware dan dikembalikan sebagai HTTP 404
        throw new NotFoundException("Resource dengan ID tersebut tidak ditemukan.");
    }

    /// <summary>
    /// FUNGSI METHOD: Mensimulasikan konflik data (HTTP 409 Conflict).
    /// 
    /// KAPAN INI TERJADI DI DUNIA NYATA:
    /// Misalnya mencoba mendaftar dengan email yang sudah terdaftar di sistem.
    /// </summary>
    [HttpGet("conflict")]
    public IActionResult GetConflict()
    {
        // Melempar ConflictException yang akan ditangkap middleware dan dikembalikan sebagai HTTP 409
        throw new ConflictException("Data dengan nama tersebut sudah ada di database.");
    }

    /// <summary>
    /// FUNGSI METHOD: Mensimulasikan kesalahan server yang tidak terduga (HTTP 500 Internal Server Error).
    /// 
    /// CARA KERJA:
    /// DivideByZeroException (pembagian dengan nol) bukan tipe AppException kustom.
    /// Middleware akan menangkapnya sebagai "Exception umum" dan mengembalikan HTTP 500 
    /// dengan pesan generik tanpa mengekspos detail internal server ke klien (aman untuk produksi).
    /// </summary>
    [HttpGet("server-error")]
    public IActionResult GetServerError()
    {
        // Menyimulasikan bug pembagian dengan nol (runtime exception tidak terduga)
        int a = 10;
        int b = 0;
        int c = a / b; // Akan melempar DivideByZeroException di sini
        return Ok(c);
    }
}
