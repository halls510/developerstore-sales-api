﻿using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an item in a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale identifier (foreign key).
    /// </summary>
    public Guid SaleId { get; set; }  // 🔹 Corrigindo para referência correta na chave composta

    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public Guid ProductId { get; set; } // External Identity for Product

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount applied.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets the total price for this item (Quantity * UnitPrice - Discount).
    /// </summary>
    public decimal Total => (Quantity * UnitPrice) - Discount;
}
