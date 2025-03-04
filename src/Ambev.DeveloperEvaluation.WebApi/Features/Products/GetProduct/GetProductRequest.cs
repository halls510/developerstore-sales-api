namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

/// <summary>
/// Request model for getting a product by ID
/// </summary>
public class GetProductRequest
{
    /// <summary>
    /// The unique identifier of the product to retrieve
    /// </summary>
    public int Id { get; set; }
}
