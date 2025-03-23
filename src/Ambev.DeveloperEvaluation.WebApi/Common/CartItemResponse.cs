using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class CartItemResponse
{
    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the discount applied.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product when added to the cart.
    /// This ensures that price changes do not affect historical cart entries.
    /// </summary>
    public decimal UnitPrice { get; set; } 

    public decimal Total { get; set; } 
}
