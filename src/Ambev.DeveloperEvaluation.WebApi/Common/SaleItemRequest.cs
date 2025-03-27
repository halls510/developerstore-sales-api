namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Represents an item in the sale request.
/// </summary>
public class SaleItemRequest
{
    /// <summary>
    /// The product ID.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The quantity of the product.
    /// </summary>
    public int Quantity { get; set; }
}
