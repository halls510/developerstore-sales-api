using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Common;

/// <summary>
/// Represents an item in the created sale result.
/// </summary>
public class SaleItemResult
{
    public int Id { get; set; }

    /// <summary>
    /// The product ID.
    /// </summary>
    public int ProductId { get; }

    /// <summary>
    /// The product name.
    /// </summary>
    public string ProductName { get; }

    /// <summary>
    /// The quantity of the product.
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// The unit price of the product.
    /// </summary>
    public Money UnitPrice { get; }

    /// <summary>
    /// The discount applied to the product.
    /// </summary>
    public Money Discount { get; }

    /// <summary>
    /// The total price after applying discounts.
    /// </summary>
    public Money Total { get; }

    /// <summary>
    /// The status of the sale item.
    /// </summary>
    public string Status { get; }

    public SaleItemResult() { }

    /// <summary>
    /// Constructor for sale item result.
    /// </summary>
    public SaleItemResult(int id,int productId, string productName, int quantity, Money unitPrice, Money discount, Money total, string status)
    {
        Id = id;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        Discount = discount ?? throw new ArgumentNullException(nameof(discount));
        Total = total ?? throw new ArgumentNullException(nameof(total));
        Status = status ?? throw new ArgumentNullException(nameof(status));
    }
}
