using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Represents the response body for a successful sale creation.
/// </summary>
public class CreateSaleResponse
{
    public int SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    public string Branch { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<SaleItemResponse> Items { get; set; } = new();
}

