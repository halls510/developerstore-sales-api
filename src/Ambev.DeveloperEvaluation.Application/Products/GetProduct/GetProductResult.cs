using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

/// <summary>
/// Represents the response returned after successfully updating a product.
/// </summary>
/// <remarks>
/// This response contains the unique identifier of the newly updated product,
/// along with relevant details such as title, price, description, category, image, and rating.
/// </remarks>
public class GetProductResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly updated product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the product.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the product using Money value object.
    /// </summary>
    public Money Price { get; set; } = new Money(0);

    /// <summary>
    /// Gets or sets the description of the product.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category name of the product.
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the image URL of the product.
    /// </summary>
    public string Image { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the rating details of the product.
    /// </summary>
    public RatingResult Rating { get; set; } = new RatingResult();
}