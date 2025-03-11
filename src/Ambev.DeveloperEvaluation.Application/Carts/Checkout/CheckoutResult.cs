using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

/// <summary>
/// Representa o resultado do processo de checkout.
/// </summary>
public class CheckoutResult
{
    /// <summary>
    /// Identificador único da venda gerada a partir do checkout.
    /// </summary>
    public int SaleId { get; set; }

    /// <summary>
    /// Data e hora em que a venda foi concluída.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Lista de itens incluídos na venda.
    /// </summary>
    public List<SaleItemDto> Items { get; set; } = new List<SaleItemDto>();

    /// <summary>
    /// Valor total da venda após descontos aplicados.
    /// </summary>
    public Money TotalValue { get; set; }

    /// <summary>
    /// Status da venda (por padrão, "Completed").
    /// </summary>
    public string Status { get; set; } = "Completed";
}
