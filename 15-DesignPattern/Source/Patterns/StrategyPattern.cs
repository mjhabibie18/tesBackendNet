// ============================================================
// StrategyPattern.cs — Implementasi Strategy Pattern
// ============================================================
// Mengubah algoritma/perilaku suatu objek secara dinamis
// di runtime tanpa merubah class utama.
// ============================================================

namespace TesBackendNet.DesignPattern.Patterns;

public interface IShippingStrategy
{
    decimal CalculateShippingCost(decimal weightKg);
}

/// <summary>Strategi Pengiriman Reguler (Standard)</summary>
public class RegularShipping : IShippingStrategy
{
    public decimal CalculateShippingCost(decimal weightKg) => weightKg * 10000;
}

/// <summary>Strategi Pengiriman Kilat (Express)</summary>
public class ExpressShipping : IShippingStrategy
{
    public decimal CalculateShippingCost(decimal weightKg) => weightKg * 25000;
}

/// <summary>Strategi Pengiriman Kargo (Bulk)</summary>
public class CargoShipping : IShippingStrategy
{
    public decimal CalculateShippingCost(decimal weightKg) => weightKg * 5000 + 50000; // Flat fee 50rb + 5rb/kg
}

/// <summary>
/// Context Class yang mengaplikasikan strategi.
/// </summary>
public class ShippingCalculator
{
    private IShippingStrategy _strategy;

    public ShippingCalculator(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    public void SetStrategy(IShippingStrategy strategy)
    {
        _strategy = strategy;
    }

    public decimal Calculate(decimal weightKg)
    {
        return _strategy.CalculateShippingCost(weightKg);
    }
}
