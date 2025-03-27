using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

/// <summary>
/// Represents a request to create a new cart in the system.
/// </summary>
public class CreateCartRequest
{
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
     public List<CartItemRequest> Products { get; set; } // Relacionamento 1:N com CartItem

}
