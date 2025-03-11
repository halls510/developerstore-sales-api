namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

/// <summary>
/// DTO representing an item inside a cart
/// </summary>
public class CartItemDto
{
    /// <summary>
    /// Unique identifier of the product
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Name of the product
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of the product in the cart
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit of the product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price of this item (Quantity * UnitPrice)
    /// </summary>
    public decimal TotalPrice { get; set; }
}
