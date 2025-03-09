namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Defines possible statuses of a sale.
/// </summary>
public enum SaleStatus
{
    Pending,   // Venda aguardando confirmação
    Confirmed, // Venda confirmada pelo sistema de pagamento
    Completed,   // Venda finalizada com sucesso
    Shipped,   // Venda enviada para entrega
    Delivered, // Cliente recebeu os produtos
    Cancelled  // Venda cancelada pelo cliente ou pelo sistema
}