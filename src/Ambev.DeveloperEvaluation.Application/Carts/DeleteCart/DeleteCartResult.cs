using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;


public class DeleteCartResult
{
    /// <summary>
    /// The unique identifier of the created cart
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the external user identifier.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Gets or sets the creation date of the cart.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Gets or sets the list of items in the cart.
    /// </summary>
    public List<CartItemResult> Products { get; set; } // Relacionamento 1:N com CartItem

    /// <summary>
    /// Gets or sets the status of the cart.
    /// Indicates whether the cart is active, completed, or cancelled.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CartStatus Status { get; set; }
}
