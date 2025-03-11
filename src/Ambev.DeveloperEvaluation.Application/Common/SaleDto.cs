namespace Ambev.DeveloperEvaluation.Application.Common;

/// <summary>
/// DTO para representar uma venda.
/// </summary>
public class SaleDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal TotalValue { get; set; }
    public List<SaleItemDto> Items { get; set; } = new();
}
