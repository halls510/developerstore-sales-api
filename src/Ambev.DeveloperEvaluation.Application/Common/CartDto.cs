namespace Ambev.DeveloperEvaluation.Application.Carts.Common;

/// <summary>
/// Data Transfer Object (DTO) for Cart
/// </summary>
public class CartDto
{
    /// <summary>
    /// Unique identifier of the cart
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Unique identifier of the customer who owns the cart
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// List of items in the cart
    /// </summary>
    public List<CartItemDto> Items { get; set; } = new();

    /// <summary>
    /// Total price of all items in the cart
    /// </summary>
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Date when the cart was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the cart was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
