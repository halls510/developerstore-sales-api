using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale transaction in the system.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale number (unique identifier for the sale).
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date when the sale was completed.
    /// </summary>
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the external user identifier for the client who made the purchase.
    /// </summary>
    public int CustomerId { get; set; } // External Identity for Customer

    /// <summary>
    /// Gets or sets the denormalized customer name.
    /// </summary>
    public string CustomerName { get; set; } = string.Empty; // Denormalized

    /// <summary>
    /// Gets or sets the total value of the sale.
    /// </summary>
    public decimal TotalValue { get; set; }

    /// <summary>
    /// Gets or sets the branch where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of products sold in this sale.
    /// </summary>
    public List<SaleItem> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the sale status.
    /// </summary>
    public SaleStatus Status { get; set; } = SaleStatus.Pending;
}

/// <summary>
/// Defines possible statuses of a sale.
/// </summary>
public enum SaleStatus
{
    Pending,   // Venda criada, mas ainda não confirmada
    Confirmed, // Venda confirmada pelo sistema de pagamento
    Shipped,   // Venda enviada para entrega
    Delivered, // Cliente recebeu os produtos
    Cancelled  // Venda cancelada
}