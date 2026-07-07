// ============================================================
// Nama File: Abstraction.cs — Demo Pilar OOP: Abstraksi
// Folder: 13-OOP/Source/Pillars/
// ============================================================
// 1. PENJELASAN FOLDER (OOP/Pillars):
//    - Tujuan: Mendemonstrasikan 4 pilar utama OOP: Encapsulation, Inheritance, Polymorphism, dan Abstraction.
//    - Kapan Digunakan: Sebagai referensi desain class ketika membangun sistem berorientasi objek yang bersih.
//    - Hubungan: Setiap file mendemonstrasikan satu pilar OOP secara terpisah agar mudah dipelajari.
//
// 2. PENJELASAN FILE:
//    - Fungsi & Tanggung Jawab: Mendemonstrasikan Abstraksi melalui interface payment gateway.
//    - Mengapa Diperlukan: Abstraksi adalah fondasi Dependency Inversion Principle (SOLID 'D').
//      Tanpa abstraksi, komponen bisnis bergantung langsung pada implementasi spesifik (tight coupling).
//    - Hubungan File: Dipanggil dari Program.cs modul OOP.
//    - Jika Dihapus: Tidak ada demonstrasi abstraksi fungsional.
//
// DEFINISI ABSTRAKSI:
// Abstraksi adalah proses menyembunyikan detail implementasi dan hanya mengekspos "kontrak" (apa yang bisa dilakukan).
// Di C#, abstraksi dicapai melalui:
//   - Interface: Kontrak murni (100% abstrak) — hanya method signatures, tidak ada implementasi.
//   - Abstract Class: Kontrak sebagian — boleh ada method dengan implementasi default.
// ============================================================

namespace TesBackendNet.OOP.Pillars;

/// <summary>
/// TUJUAN INTERFACE: Mendefinisikan kontrak abstrak untuk semua payment gateway.
/// 
/// PRINSIP ABSTRAKSI:
/// Kode bisnis (misalnya OrderService) tidak perlu tahu apakah menggunakan Stripe atau Midtrans.
/// Yang diketahui hanyalah: "Ada sesuatu yang bisa `ProcessPayment`."
/// Ini memungkinkan penggantian provider pembayaran tanpa mengubah satu baris pun kode bisnis.
/// 
/// PRINSIP DEPENDENCY INVERSION (SOLID-D):
/// - High-level module (OrderService) bergantung pada abstraksi (IPaymentGateway), bukan pada implementasi konkrit.
/// - Low-level module (StripeGateway, MidtransGateway) mengimplementasikan abstraksi yang sama.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// FUNGSI METHOD: Memproses pembayaran kartu kredit/debit.
    /// PARAMETER: amount (jumlah yang ditagih), cardNumber (nomor kartu pembayaran).
    /// NILAI KEMBALIAN: bool — true jika transaksi berhasil, false jika gagal.
    /// </summary>
    bool ProcessPayment(decimal amount, string cardNumber);
}

/// <summary>
/// TUJUAN CLASS: Implementasi payment gateway menggunakan layanan Stripe (internasional).
/// </summary>
public class StripeGateway : IPaymentGateway
{
    /// <summary>
    /// FUNGSI METHOD: Memproses pembayaran melalui Stripe API.
    /// </summary>
    public bool ProcessPayment(decimal amount, string cardNumber)
    {
        Console.WriteLine($"[Stripe] Memproses kartu {cardNumber} senilai Rp{amount:N0}");
        return true; // Simulasi selalu berhasil
    }
}

/// <summary>
/// TUJUAN CLASS: Implementasi payment gateway menggunakan layanan Midtrans (Indonesia).
/// POLIMORFISME: Kedua class (Stripe & Midtrans) dapat diperlakukan sebagai IPaymentGateway yang sama.
/// </summary>
public class MidtransGateway : IPaymentGateway
{
    /// <summary>
    /// FUNGSI METHOD: Memproses pembayaran melalui Midtrans API.
    /// </summary>
    public bool ProcessPayment(decimal amount, string cardNumber)
    {
        Console.WriteLine($"[Midtrans] Menghubungkan ke API Midtrans untuk kartu {cardNumber} senilai Rp{amount:N0}");
        return true;
    }
}
