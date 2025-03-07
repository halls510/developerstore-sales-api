﻿using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.DeleteCart;

/// <summary>
/// API response model for DeleteCart operation
/// </summary>
public class DeleteCartResponse
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
    
}
