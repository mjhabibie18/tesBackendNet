// ============================================================
// Nama File: FactoryPattern.cs — Implementasi Simple Factory Pattern
// Folder: 15-DesignPattern/Source/Patterns/
// ============================================================
// 1. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendemonstrasikan Simple Factory Pattern untuk membuat objek gateway pembayaran
//      berdasarkan string input tanpa klien perlu mengetahui kelas konkrit mana yang diinstansiasi.
//    - Mengapa Diperlukan: Saat sistem mendukung banyak provider (OVO, GoPay, Dana, dll.), klien tidak harus
//      tahu class mana yang perlu diinstansiasi secara langsung.
//    - Hubungan File: PaymentGatewayFactory menggunakan interface IPaymentGateway dan semua implementasinya.
//    - Jika Dihapus: Tidak ada demonstrasi Factory Pattern pada modul Design Pattern.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

/// <summary>
/// TUJUAN INTERFACE:
/// Kontrak perilaku untuk semua gateway pembayaran yang didukung oleh sistem.
/// 
/// ALASAN MENGGUNAKAN INTERFACE DI SINI:
/// Interface ini menjadi titik abstraksi. Klien hanya bergantung pada `IPaymentGateway`, 
/// bukan pada implementasi konkrit (OVO, GoPay, dll.) secara langsung.
/// Ini memungkinkan penambahan metode pembayaran baru tanpa mengubah kode klien (OCP).
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// FUNGSI METHOD: Memproses pembayaran sejumlah uang tertentu.
    /// PARAMETER: amount (jumlah yang dibayarkan dalam Rupiah).
    /// </summary>
    void Pay(decimal amount);
}

/// <summary>
/// TUJUAN CLASS: Implementasi gateway OVO.
/// `:N0` dalam format string adalah format number dengan pemisah ribuan tanpa desimal (misal: 150.000).
/// </summary>
public class OVO : IPaymentGateway
{
    public void Pay(decimal amount) => Console.WriteLine($"[OVO] Pembayaran berhasil diproses senilai Rp{amount:N0}");
}

/// <summary>
/// TUJUAN CLASS: Implementasi gateway GoPay.
/// </summary>
public class GoPay : IPaymentGateway
{
    public void Pay(decimal amount) => Console.WriteLine($"[GoPay] Pembayaran berhasil diproses senilai Rp{amount:N0}");
}

/// <summary>
/// TUJUAN CLASS: Implementasi gateway Dana.
/// </summary>
public class Dana : IPaymentGateway
{
    public void Pay(decimal amount) => Console.WriteLine($"[Dana] Pembayaran berhasil diproses senilai Rp{amount:N0}");
}

/// <summary>
/// TUJUAN CLASS:
/// Factory Class — Bertanggung jawab menginstansiasi objek gateway pembayaran yang tepat berdasarkan string type.
/// 
/// KEUNTUNGAN FACTORY PATTERN:
/// - Klien hanya memanggil `PaymentGatewayFactory.Create("gopay")` dan tidak perlu tahu class GoPay ada.
/// - Menambah gateway baru hanya memerlukan tambahan case di Factory ini, tanpa mengubah kode klien manapun.
/// 
/// BARIS KODE PENTING (`switch expression`):
/// `type.ToLower() switch { "gopay" => new GoPay(), ... }` adalah switch expression (C# 8+).
/// Berbeda dari switch statement tradisional, switch expression dapat mengembalikan nilai langsung (expression-bodied).
/// Lebih ringkas dan mudah dibaca dibandingkan if-else atau switch statement klasik.
/// `_` di paling bawah adalah discard pattern — menangkap semua input yang tidak cocok.
/// </summary>
public static class PaymentGatewayFactory
{
    /// <summary>
    /// FUNGSI METHOD: Membuat dan mengembalikan objek gateway pembayaran yang sesuai.
    /// PARAMETER: type (nama metode pembayaran: "gopay", "ovo", "dana").
    /// NILAI KEMBALIAN: IPaymentGateway — referensi ke objek konkrit, dibungkus dalam interface.
    /// EXCEPTION: ArgumentException jika type tidak dikenali.
    /// </summary>
    public static IPaymentGateway Create(string type)
    {
        // Switch expression (C# 8+): Menentukan kelas yang diinstansiasi berdasarkan string type
        return type.ToLower() switch
        {
            "gopay" => new GoPay(),
            "ovo"   => new OVO(),
            "dana"  => new Dana(),
            // Discard pattern: jika tidak ada yang cocok, lempar exception informatif
            _ => throw new ArgumentException($"Metode pembayaran '{type}' tidak didukung.")
        };
    }
}
