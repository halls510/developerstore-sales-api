using System;

namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

/// <summary>
/// Representa um valor monetário como um Value Object.
/// </summary>
public record Money
{
    public decimal Amount { get; }

    /// <summary>
    /// Construtor para garantir que o valor nunca seja negativo.
    /// </summary>
    /// <param name="amount">Valor do dinheiro</param>
    public Money(decimal amount)
    {
        if (amount >= 0)
        {
            Amount = Math.Round(amount, 2, MidpointRounding.AwayFromZero); // Evita problemas de precisão
        }        
    }

    /// <summary>
    /// Sobrecarga para operações matemáticas
    /// </summary>
    public static Money operator +(Money a, Money b) => new Money(a.Amount + b.Amount);
    public static Money operator -(Money a, Money b) => new Money(a.Amount - b.Amount);
    public static Money operator *(Money a, decimal multiplier) => new Money(a.Amount * multiplier);
    public static Money operator /(Money a, decimal divisor) => divisor == 0
        ? throw new DivideByZeroException("Divisão por zero não permitida.")
        : new Money(a.Amount / divisor);

    /// <summary>
    /// Comparações entre valores
    /// </summary>
    public static bool operator >(Money a, Money b) => a.Amount > b.Amount;
    public static bool operator <(Money a, Money b) => a.Amount < b.Amount;
    public static bool operator >=(Money a, Money b) => a.Amount >= b.Amount;
    public static bool operator <=(Money a, Money b) => a.Amount <= b.Amount;

    /// <summary>
    /// Representação formatada do dinheiro
    /// </summary>
    public override string ToString() => Amount.ToString("C");

    /// <summary>
    /// Permite converter implicitamente um decimal para Money
    /// </summary>
    public static implicit operator Money(decimal amount) => new Money(amount);

    /// <summary>
    /// Permite converter um Money para decimal
    /// </summary>
    public static implicit operator decimal(Money money) => money.Amount;
}
