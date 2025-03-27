using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemResult
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public Money UnitPrice { get; set; }
    public Money Discount { get; set; }
    public Money Total { get; set; }
    public SaleItemStatus Status { get; set; }
}
