using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Common;

public class CartItemResult
{
    /// <summary>
    /// The unique identifier of the created cart item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the associated cart ID.
    /// </summary>
    public int CartId { get; set; } 

    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public int ProductId { get; set; } 

    /// <summary>
    /// Gets or sets the product name at the time of adding to cart.
    /// This field is denormalized to preserve historical data.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Gets the discount applied.
    /// </summary>
    public Money Discount { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product when added to the cart.
    /// This ensures that price changes do not affect historical cart entries.
    /// </summary>
    public Money UnitPrice { get; set; } = new Money(0); // Agora é `Money`

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the total cost of the cart item.
    /// </summary>
    public Money Total => new Money(0); // Agora retorna um `Money`
}
