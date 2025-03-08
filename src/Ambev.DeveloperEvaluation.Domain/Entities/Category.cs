using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a product category within the same domain.
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Gets the default category ID ("Sem Categoria").
    /// </summary>
    public static readonly int DefaultCategoryId = 1;

    /// <summary>
    /// Gets the default category name.
    /// </summary>
    public static readonly string DefaultCategoryName = "Sem Categoria";

    /// <summary>
    /// Gets or sets the category name.
    /// Must be unique and have a maximum length of 100 characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Navigation property to the related products.
    /// </summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
