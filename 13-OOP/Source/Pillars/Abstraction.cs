// ============================================================
// Abstraction.cs — Demo Abstraksi
// ============================================================

namespace TesBackendNet.OOP.Pillars;

/// <summary>
/// Kontrak Abstraksi. Client tidak perlu tahu bagaimana pembayaran diproses,
/// cukup panggil method dari interface ini.
/// </summary>
public interface IPaymentGateway
{
    bool ProcessPayment(decimal amount, string cardNumber);
}

public class StripeGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount, string cardNumber)
    {
        Console.WriteLine($"[Stripe] Memproses kartu {cardNumber} senilai Rp{amount:N0}");
        return true;
    }
}

public class MidtransGateway : IPaymentGateway
{
    public bool ProcessPayment(decimal amount, string cardNumber)
    {
        Console.WriteLine($"[Midtrans] Menghubungkan ke API Midtrans untuk kartu {cardNumber} senilai Rp{amount:N0}");
        return true;
    }
}
