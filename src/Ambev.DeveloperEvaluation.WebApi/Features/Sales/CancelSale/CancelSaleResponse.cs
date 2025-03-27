using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleResponse
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
    public List<SaleItemResponse> Items { get; set; }

    public CancelSaleResponse()
    {
        Items = new List<SaleItemResponse>();
    }

    /// <summary>
    /// Constructor for the sale result.
    /// </summary>
    public CancelSaleResponse(int saleId, string saleNumber, DateTime saleDate, int customerId, string customerName, Money totalValue, string branch, string status, List<SaleItemResponse> items)
    {
        SaleId = saleId;
        SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName ?? throw new ArgumentNullException(nameof(customerName));
        TotalValue = totalValue ?? throw new ArgumentNullException(nameof(totalValue));
        Branch = branch ?? throw new ArgumentNullException(nameof(branch));
        Status = status ?? throw new ArgumentNullException(nameof(status));
        Items = items ?? new List<SaleItemResponse>();
    }
}
