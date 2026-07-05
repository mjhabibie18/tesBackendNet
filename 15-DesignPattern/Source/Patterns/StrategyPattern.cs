// ============================================================
// Nama File: StrategyPattern.cs — Implementasi Strategy Pattern
// Folder: 15-DesignPattern/Source/Patterns/
// ============================================================
// 1. PENJELASAN FOLDER (DesignPattern/Patterns):
//    - Tujuan: Menyediakan implementasi konkret dari design pattern yang paling sering muncul di technical interview dan kode produksi.
//    - Kapan Digunakan: Saat arsitektur membutuhkan fleksibilitas pemilihan algoritma atau perilaku secara dinamis.
//    - Hubungan: Pattern ini diterapkan di banyak modul lain, seperti cara Strategy digunakan di sistem pembayaran, ORM, dan query builder.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendemonstrasikan Strategy Pattern — pola desain yang memisahkan algoritma dari kliennya sehingga dapat ditukar di runtime.
//    - Mengapa Diperlukan: Tanpa Strategy Pattern, setiap perubahan algoritma pengiriman memerlukan modifikasi pada class utama, melanggar Open/Closed Principle.
//    - Jika Dihapus: Tidak ada demonstrasi runtime algorithm switching pada modul Design Pattern.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

/// <summary>
/// TUJUAN INTERFACE:
/// Kontrak perilaku tunggal yang harus diimplementasikan oleh setiap strategi pengiriman.
/// 
/// ALASAN MENGGUNAKAN INTERFACE (Bukan Abstract Class):
/// Interface lebih fleksibel karena C# tidak mendukung multiple inheritance, tetapi mendukung implementasi banyak interface.
/// Sebuah class strategi dapat mengimplementasikan lebih dari satu interface sekaligus (misal: IShippingStrategy sekaligus IDiscountStrategy).
/// </summary>
public interface IShippingStrategy
{
    /// <summary>
    /// FUNGSI METHOD: Menghitung biaya pengiriman berdasarkan berat barang.
    /// PARAMETER: weightKg (berat dalam kilogram).
    /// NILAI KEMBALIAN: decimal (biaya pengiriman dalam Rupiah).
    /// </summary>
    decimal CalculateShippingCost(decimal weightKg);
}

/// <summary>
/// TUJUAN CLASS: Strategi pengiriman reguler (tarif paling terjangkau).
/// FORMULA: Rp 10.000 per kilogram.
/// KAPAN DIGUNAKAN: Dipilih saat pengguna memilih opsi pengiriman standar.
/// </summary>
public class RegularShipping : IShippingStrategy
{
    public decimal CalculateShippingCost(decimal weightKg) => weightKg * 10000;
}

/// <summary>
/// TUJUAN CLASS: Strategi pengiriman kilat (cepat, lebih mahal).
/// FORMULA: Rp 25.000 per kilogram.
/// KAPAN DIGUNAKAN: Dipilih saat pengguna membutuhkan pengiriman hari yang sama (same-day delivery).
/// </summary>
public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateShippingCost(decimal weightKg) => weightKg * 25000;
}

/// <summary>
/// TUJUAN CLASS: Strategi pengiriman kargo (volume besar, tarif per kilogram lebih murah + biaya dasar tetap).
/// FORMULA: Biaya tetap Rp 50.000 + Rp 5.000 per kilogram.
/// KAPAN DIGUNAKAN: Untuk pengiriman produk dalam jumlah besar (grosir/B2B).
/// </summary>
public class CargoShipping : IShippingStrategy
{
    public decimal CalculateShippingCost(decimal weightKg) => weightKg * 5000 + 50000; 
}

/// <summary>
/// TUJUAN CLASS:
/// Context Class yang berperan sebagai klien yang menggunakan strategi pengiriman.
/// 
/// KUNCI STRATEGY PATTERN:
/// - Class ini menyimpan referensi ke interface IShippingStrategy (bukan implementasi konkritnya).
/// - Algoritma yang digunakan dapat diganti (ditukar) saat runtime melalui method SetStrategy().
/// - Context Class tidak perlu diubah ketika ada strategi baru (misal: Drone Delivery) — cukup tambahkan implementasi baru.
/// 
/// KEUNTUNGAN DIBANDING IF-ELSE:
/// Bayangkan tanpa Strategy Pattern:
///   if (type == "regular") cost = weight * 10000;
///   else if (type == "express") cost = weight * 25000;
///   else if (type == "cargo") cost = ...;
/// Setiap strategi baru memerlukan perubahan pada class ini (melanggar Open/Closed Principle).
/// </summary>
public class ShippingCalculator
{
    // Menyimpan strategi yang sedang aktif — bertipe interface, bukan konkrit
    private IShippingStrategy _strategy;

    /// <summary>
    /// CONSTRUCTOR: Strategi awal yang digunakan saat kalkulator dibuat pertama kali.
    /// </summary>
    public ShippingCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// FUNGSI METHOD: Mengganti strategi pengiriman secara dinamis saat runtime.
    /// KAPAN DIGUNAKAN: Saat pengguna memilih metode pengiriman berbeda di halaman checkout.
    /// </summary>
    public void SetStrategy(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    /// <summary>
    /// FUNGSI METHOD: Menghitung biaya menggunakan strategi yang sedang aktif.
    /// ALUR EKSEKUSI: Mendelegasikan kalkulasi ke implementasi konkrit yang tersimpan di _strategy.
    /// </summary>
    public decimal Calculate(decimal weightKg)
    {
        return _strategy.CalculateShippingCost(weightKg);
    }
}
