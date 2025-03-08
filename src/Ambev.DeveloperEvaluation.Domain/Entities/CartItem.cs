using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item in a shopping cart.
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the associated cart ID.
    /// </summary>
    public int CartId { get; set; } // Chave estrangeira para Cart

    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public int ProductId { get; set; } // External Identity for Product

    /// <summary>
    /// Gets or sets the product name at the time of adding to cart.
    /// This field is denormalized to preserve historical data.
    /// </summary>
    public string ProductName { get; set; } = string.Empty; // Denormalized product name

    /// <summary>
    /// Gets or sets the unit price of the product when added to the cart.
    /// This ensures that price changes do not affect historical cart entries.
    /// </summary>
    public decimal UnitPrice { get; set; } // Denormalized price

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the total cost of the cart item.
    /// </summary>
    public decimal Total => UnitPrice * Quantity;
}
