namespace Ambev.DeveloperEvaluation.Domain.Enums;

/// <summary>
/// Defines possible statuses of a sale.
/// </summary>
public enum SaleStatus
{
    Pending,       // Aguardando pagamento ou aprovação
    Processing,    // Pedido processado e aguardando confirmação do pagamento
    Confirmed,     // Pagamento confirmado e pedido pronto para envio
    Completed,     // Venda finalizada com sucesso
    Shipped,       // Pedido enviado para entrega
    Delivered,     // Cliente recebeu os produtos
    Cancelled,     // Venda cancelada pelo cliente ou pelo sistema
    Failed,        // Falha no pagamento ou erro no processamento
    Refunded       // Pedido reembolsado ao cliente
}
