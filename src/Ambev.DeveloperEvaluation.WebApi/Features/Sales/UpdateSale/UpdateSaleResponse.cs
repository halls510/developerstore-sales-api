using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Represents the response body for a successful sale updating.
/// </summary>
public class UpdateSaleResponse
{
    public int SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Money TotalValue { get; set; } = new(0);
    public string Branch { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<SaleItemResponse> Items { get; set; } = new();
}

