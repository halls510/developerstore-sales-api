using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

/// <summary>
/// Represents an item in the sale response.
/// </summary>
public class SaleItemResponse
{ 
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } 
    public decimal Discount { get; set; } 
    public decimal Total { get; set; } 
    public string Status { get; set; } = string.Empty;
}
