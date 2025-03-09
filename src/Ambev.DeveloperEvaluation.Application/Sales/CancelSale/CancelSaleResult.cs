using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleResult
{
    /// <summary>
    /// The unique identifier of the sale.
    /// </summary>
    public int SaleId { get; set; }

    /// <summary>
    /// The unique sale number.
    /// </summary>
    public string SaleNumber { get; set; }

    /// <summary>
    /// The date when the sale was completed.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The customer ID associated with the sale.
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// The name of the customer.
    /// </summary>
    public string CustomerName { get; set; }

    /// <summary>
    /// The total value of the sale.
    /// </summary>
    public Money TotalValue { get; set; }

    /// <summary>
    /// The branch where the sale occurred.
    /// </summary>
    public string Branch { get; set; }

    /// <summary>
    /// The status of the sale.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// The list of items included in the sale.
    /// </summary>
    public List<SaleItemResult> Items { get; set; }

    public CancelSaleResult()
    {
        Items = new List<SaleItemResult>();
    }

    /// <summary>
    /// Constructor for the sale result.
    /// </summary>
    public CancelSaleResult(int saleId, string saleNumber, DateTime saleDate, int customerId, string customerName, Money totalValue, string branch, string status, List<SaleItemResult> items)
    {
        SaleId = saleId;
        SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        TotalValue = totalValue ?? throw new ArgumentNullException(nameof(totalValue));
        Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        Status = status ?? throw new ArgumentNullException(nameof(status));
        Items = items ?? new List<SaleItemResult>();
    }
}
