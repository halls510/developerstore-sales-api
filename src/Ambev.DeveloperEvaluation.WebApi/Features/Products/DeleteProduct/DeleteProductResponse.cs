using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Common;
using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;

/// <summary>
/// API response model for DeleteProduct operation
/// </summary>
public class DeleteProductResponse
{
     /// <summary>
     /// The unique identifier of the created product
     /// </summary>
     public int Id { get; set; }

     /// <summary>
     /// Gets or sets the title of the product. Must be a non-empty string.
     /// </summary>
     public string Title { get; set; } = string.Empty;

     /// <summary>
     /// Gets or sets the price of the product. Must be a positive decimal value.
     /// </summary>
     public decimal Price { get; set; }

     /// <summary>
     /// Gets or sets the product description. Should provide detailed information.
     /// </summary>
     public string Description { get; set; } = string.Empty;

     /// <summary>
     /// Gets or sets the category of the product as a string.
     /// </summary>
     public string Category { get; set; } = string.Empty;

     /// <summary>
     /// Gets or sets the image URL of the product.
     /// </summary>
     public string Image { get; set; } = string.Empty;

     /// <summary>
     /// Gets or sets the rating details of the product.
     /// </summary>
     public RatingResponse Rating { get; set; } = new RatingResponse();
    
}
