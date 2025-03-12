namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Defines possible statuses of a sale item.
/// </summary>
public enum SaleItemStatus
{
    Active,       // Item ativo na venda
    Cancelled,    // Item cancelado pelo cliente ou sistema
    Returned,     // Item devolvido após a compra
    OutOfStock,   // Item estava no pedido, mas ficou sem estoque antes do envio
    Shipped,      // Item enviado para o cliente
    Delivered     // Cliente recebeu o item
}
