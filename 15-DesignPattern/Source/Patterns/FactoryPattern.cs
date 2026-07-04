// ============================================================
// FactoryPattern.cs — Implementasi Simple Factory
// ============================================================
// Factory mengisolasi instansiasi objek berdasarkan parameter
// agar client tidak terikat pada sub-class konkrit.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

public interface IPaymentGateway
{
    void Pay(decimal amount);
}

public class OVO : IPaymentGateway
{
    public void Pay(decimal amount) => Console.WriteLine($"[OVO] Pembayaran berhasil diproses senilai Rp{amount:N0}");
}

public class GoPay : IPaymentGateway
{
    public void Pay(decimal amount) => Console.WriteLine($"[GoPay] Pembayaran berhasil diproses senilai Rp{amount:N0}");
}

public class Dana : IPaymentGateway
{
    public void Pay(decimal amount) => Console.WriteLine($"[Dana] Pembayaran berhasil diproses senilai Rp{amount:N0}");
}

/// <summary>
/// Factory Class untuk instansiasi e-wallet gateway.
/// </summary>
public static class PaymentGatewayFactory
{
    public static IPaymentGateway Create(string type)
    {
        return type.ToLower() switch
        {
            "gopay" => new GoPay(),
            "ovo" => new OVO(),
            "dana" => new Dana(),
            _ => throw new ArgumentException($"Metode pembayaran '{type}' tidak didukung.")
        };
    }
}
