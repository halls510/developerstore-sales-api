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
    /// Gets or sets the user’s full name at the time of cart creation.
    /// This field is denormalized to preserve the original user information.
    /// </summary>
    public string UserName { get; set; } = string.Empty; // Denormalized user name

    /// <summary>
    /// Gets or sets the creation date of the cart.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of items in the cart.
    /// </summary>
    public List<CartItem> Items { get; set; } = new(); // Relacionamento 1:N com CartItem
}
