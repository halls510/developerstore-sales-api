namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;

/// <summary>
/// Request model for deleting a product
/// </summary>
public class DeleteProductRequest
{
    /// <summary>
    /// The unique identifier of the product to delete
    /// </summary>
    public int Id { get; set; }
}
