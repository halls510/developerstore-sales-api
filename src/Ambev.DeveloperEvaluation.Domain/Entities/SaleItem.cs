using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item in a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets the sale identifier (foreign key).
    /// </summary>
    public int SaleId { get; set; }

    /// <summary>
    /// Gets the external product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets the product name.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public Money UnitPrice { get; set; }

    /// <summary>
    /// Gets the discount applied.
    /// </summary>
    public Money Discount { get; set; }    

    /// <summary>
    /// Gets the total price for this item (Quantity * UnitPrice - Discount).
    /// </summary>
    public Money Total { get; set; }

    /// <summary>
    /// Gets the status of the sale item.
    /// </summary>
    public SaleItemStatus Status { get; private set; }

    public SaleItem()
    {
        Status = SaleItemStatus.Active;
        Quantity = 0;
        UnitPrice = new Money(0);
        Discount = new Money(0);
        Total = new Money(0);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="productName"></param>
    /// <param name="quantity"></param>
    /// <param name="unitPrice"></param>
    /// <param name="discount"></param>
    /// <param name="total"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SaleItem(int productId, string productName, int quantity, Money unitPrice, Money discount, Money total)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
                
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
        Status = SaleItemStatus.Active;
        Discount = discount;
        Total = total;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class.
    /// </summary>
    /// <param name="itemId"></param>
    /// <param name="saleId"></param>
    /// <param name="productId"></param>
    /// <param name="productName"></param>
    /// <param name="quantity"></param>
    /// <param name="unitPrice"></param>
    /// <param name="discount"></param>
    /// <param name="total"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SaleItem(int itemId,int saleId,int productId, string productName, int quantity, Money unitPrice, Money discount, Money total)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        Id = itemId;
        SaleId = saleId;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
        Status = SaleItemStatus.Active;
        Discount = discount;
        Total = total;
    }

    /// <summary>
    /// Cancels the item, updating its status.
    /// </summary>
    public void Cancel()
    {
        if (Status != SaleItemStatus.Active)
            throw new InvalidOperationException("Only active items can be cancelled.");

        Status = SaleItemStatus.Cancelled;
    }
}

