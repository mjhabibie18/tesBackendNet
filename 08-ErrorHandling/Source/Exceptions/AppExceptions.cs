// ============================================================
// Nama File: AppExceptions.cs — Kumpulan Custom Exception Classes
// Folder: 08-ErrorHandling/Source/Exceptions/
// ============================================================
// 1. PENJELASAN FOLDER (ErrorHandling/Exceptions):
//    - Tujuan: Mendefinisikan hierarki kelas exception kustom yang mencerminkan kesalahan bisnis spesifik.
//    - Kapan Digunakan: Saat logika bisnis (Service Layer) ingin melaporkan kondisi kesalahan yang bermakna ke middleware handler.
//    - Hubungan: Exception di sini ditangkap oleh GlobalExceptionMiddleware.cs dan dipetakan ke HTTP status code yang sesuai.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Menyediakan kelas exception hierarkis yang dapat dilempar dari mana saja di aplikasi.
//    - Mengapa Diperlukan: Menggunakan tipe exception spesifik (bukan Exception umum) memungkinkan middleware
//      memilih HTTP status code yang tepat secara otomatis berdasarkan tipe exception yang ditangkap.
//    - Hubungan File: Digunakan di DemoController.cs dan dikenali di GlobalExceptionMiddleware.cs.
//    - Jika Dihapus: Penanganan error menjadi tidak terstruktur; semua kesalahan akan menghasilkan HTTP 500.
// ============================================================

namespace TesBackendNet.ErrorHandling.Exceptions;

/// <summary>
/// TUJUAN CLASS:
/// Kelas dasar abstrak (base abstract class) untuk semua custom exception dalam aplikasi ini.
/// 
/// ALASAN MENGGUNAKAN ABSTRACT CLASS:
/// - Kita tidak ingin developer dapat melempar AppException secara langsung (tidak ada konteks spesifik).
/// - Kelas turunan (NotFoundException, ConflictException, dll.) yang mewarisi kelas ini otomatis membawa
///   properti StatusCode yang dapat dibaca oleh GlobalExceptionMiddleware untuk menentukan HTTP response code.
/// 
/// ALASAN MEWARISI System.Exception:
/// Dengan mewarisi `Exception`, kelas ini sepenuhnya kompatibel dengan mekanisme try-catch C# bawaan,
/// termasuk penangkapan stack trace, inner exception, dan pesan error.
/// 
/// PRINSIP OOP yang Diterapkan:
/// - Inheritance: Semua exception kustom mewarisi kelas ini untuk mendapatkan properti StatusCode.
/// - Polymorphism: Middleware dapat menangkap satu tipe (AppException) dan memproses semua turunannya.
/// </summary>
public abstract class AppException : Exception
{
    /// <summary>
    /// FUNGSI PROPERTY: Menyimpan HTTP status code yang tepat untuk exception ini.
    /// Dibaca oleh GlobalExceptionMiddleware untuk mengisi `context.Response.StatusCode`.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// CONSTRUCTOR: Menerima pesan error dan status code HTTP yang sesuai dari kelas turunan.
    /// Keyword `base(message)` meneruskan pesan ke konstruktor System.Exception agar dapat dibaca via .Message.
    /// </summary>
    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

/// <summary>
/// TUJUAN CLASS:
/// Dilempar ketika sebuah resource (data) yang diminta tidak ditemukan di database.
/// 
/// HTTP Status Code: 404 Not Found.
/// Kapan Digunakan: Saat `Repository.GetByIdAsync(id)` mengembalikan null, atau entity tidak ada.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound)
    {
    }
}

/// <summary>
/// TUJUAN CLASS:
/// Dilempar ketika terjadi konflik state data, misalnya mencoba mendaftarkan email yang sudah ada.
/// 
/// HTTP Status Code: 409 Conflict.
/// Kapan Digunakan: Saat validasi bisnis menemukan duplikat data.
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, StatusCodes.Status409Conflict)
    {
    }
}

/// <summary>
/// TUJUAN CLASS:
/// Dilempar ketika parameter atau payload request tidak memenuhi aturan validasi bisnis.
/// 
/// HTTP Status Code: 400 Bad Request.
/// Kapan Digunakan: Saat format data benar secara sintaksis (JSON valid), namun tidak sesuai aturan domain.
/// </summary>
public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest)
    {
    }
}
