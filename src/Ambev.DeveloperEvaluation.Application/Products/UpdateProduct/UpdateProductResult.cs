using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

/// <summary>
/// Represents the response returned after successfully creating a new product.
/// </summary>
/// <remarks>
/// This response contains the unique identifier of the newly created product,
/// along with relevant details such as title, price, description, category, image, and rating.
/// </remarks>
public class UpdateProductResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the product.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the product.
    /// </summary>
    public decimal Price { get; set; }

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
