using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents the request body for creating a sale.
/// </summary>
public class UpdateSaleRequest
{

    /// <summary>
    /// The customer ID making the purchase.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The list of items in the sale.
    /// </summary>
    public List<SaleItemRequest> Items { get; set; } = new();
}
