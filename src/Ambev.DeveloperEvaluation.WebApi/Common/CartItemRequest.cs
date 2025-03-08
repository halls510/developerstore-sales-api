namespace Ambev.DeveloperEvaluation.WebApi.Common;

public class CartItemRequest
{
    /// <summary>
    /// Gets or sets the external product identifier.
    /// </summary>
    public int ProductId { get; set; } 

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }
}
