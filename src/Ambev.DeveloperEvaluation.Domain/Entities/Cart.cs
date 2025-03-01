using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a shopping cart in the system.
/// </summary>
public class Cart : BaseEntity
{
    /// <summary>
    /// Gets or sets the external user identifier.
    /// </summary>
    public Guid UserId { get; set; } // External Identity for User

    /// <summary>
    /// Gets or sets the date when the cart was created.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of products in the cart.
    /// </summary>
    public List<CartItem> Products { get; set; } = new();
}

/// <summary>
/// Represents a product inside a shopping cart.
/// </summary>
public class CartItem
{
    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public Guid ProductId { get; set; } // External Identity for Product

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }
}
