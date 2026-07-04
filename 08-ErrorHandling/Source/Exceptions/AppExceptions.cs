// ============================================================
// AppExceptions.cs — Kumpulan Custom Exceptions
// ============================================================
// Custom Exception membuat penanganan error lebih terstruktur.
// Middleware dapat menangkap tipe exception ini dan memetakan ke
// status code HTTP yang sesuai.
// ============================================================

namespace TesBackendNet.ErrorHandling.Exceptions;

/// <summary>Base custom exception class untuk aplikasi kita.</summary>
public abstract class AppException : Exception
{
    public int StatusCode { get; }

    protected AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

/// <summary>Dilempar jika resource yang dicari tidak ditemukan (404).</summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound)
    {
    }
}

/// <summary>Dilempar jika terjadi konflik state data (409).</summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message, StatusCodes.Status409Conflict)
    {
    }
}

/// <summary>Dilempar jika terjadi kesalahan validasi bisnis (400).</summary>
public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest)
    {
    }
}
