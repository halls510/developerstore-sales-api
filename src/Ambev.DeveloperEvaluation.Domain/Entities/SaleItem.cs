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

    // Campo privado para armazenar `Total`
    private Money _total = new Money(0);

    /// <summary>
    /// Gets the total price for this item (Quantity * UnitPrice - Discount).
    /// </summary>
    public Money Total
    {
        get => new Money((Quantity * UnitPrice.Amount) - Discount.Amount);
        private set => _total = value; // Setter privado para o EF Core
    }

    /// <summary>
    /// Gets the status of the sale item.
    /// </summary>
    public SaleItemStatus Status { get; private set; }

    public SaleItem()
    {
        Status = SaleItemStatus.Active;
        Discount = new Money(0);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class.
    /// </summary>
    public SaleItem(int saleId, int productId, string productName, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        SaleId = saleId;
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice;
        Status = SaleItemStatus.Active;
        Discount = new Money(0);
    }

    ///// <summary>
    ///// Applies a new discount to the sale item.
    ///// </summary>
    //public void ApplyDiscount(Money newDiscount)
    //{
    //    if (newDiscount.Amount < 0)
    //        throw new ArgumentException("Discount cannot be negative.", nameof(newDiscount));

    //    if (newDiscount.Amount > (UnitPrice.Amount * Quantity))
    //        throw new ArgumentException("Discount cannot be greater than total price.");

    //    Discount = newDiscount;
    //}

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

