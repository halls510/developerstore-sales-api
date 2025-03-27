namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Represents the possible statuses of a shopping cart.
/// </summary>
public enum CartStatus
{
    Active,      // O cliente ainda está adicionando itens
    CheckedOut,  // O checkout foi realizado, criando uma venda
    Completed,   // A venda foi concluída e o pedido foi processado
    Cancelled    // O carrinho foi abandonado ou cancelado
}