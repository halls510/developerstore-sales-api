namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Represents the possible statuses of a shopping cart.
/// </summary>
public enum CartStatus
{
    Active,     // Carrinho ainda em uso
    Completed,  // Venda finalizada
    Cancelled   // Carrinho cancelado ou abandonado
}