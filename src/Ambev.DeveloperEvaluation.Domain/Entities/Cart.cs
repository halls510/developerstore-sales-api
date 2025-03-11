using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a shopping cart in the system.
/// </summary>
public class Cart : BaseEntity
{
    /// <summary>
    /// Gets or sets the external user identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the user’s full name at the time of cart creation.
    /// This field is denormalized to preserve the original user information.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the creation date of the cart.
    /// </summary>
    public DateTime Date { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of items in the cart.
    /// </summary>
    public List<CartItem> Items { get; set; } = new(); // Relacionamento 1:N com CartItem

    /// <summary>
    /// Gets or sets the status of the cart.
    /// Indicates whether the cart is active, completed, or cancelled.
    /// </summary>
    public CartStatus Status { get; set; } = CartStatus.Active;

    /// <summary>
    /// Gets or sets the total price of the cart.
    /// </summary>
    public Money TotalPrice { get; set; }

    /// <summary>
    /// Marks the cart as checked out, preventing further modifications.
    /// </summary>
    public void MarkAsCheckedOut()
    {
        if (Status == CartStatus.CheckedOut)
            throw new BusinessRuleException("O carrinho já foi finalizado.");

        if (!Items.Any())
            throw new BusinessRuleException("Não é possível finalizar um carrinho vazio.");

        Status = CartStatus.CheckedOut;
    }
}