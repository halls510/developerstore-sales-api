﻿using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartResponse
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
    public List<CartItemResponse> Products { get; set; } // Relacionamento 1:N com CartItem

    /// <summary>
    /// Gets or sets the total price of the cart.
    /// </summary>
    public decimal TotalPrice { get; set; }
}
