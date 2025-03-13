using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesResult
{
    /// <summary>
    /// The list of Sales
    /// </summary>
    public List<GetSaleResult> Sales { get; set; } = new List<GetSaleResult>();

    /// <summary>
    /// The total number of Sales in the system
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// The current page number
    /// </summary>
    public int CurrentPage { get; set; }

    /// <summary>
    /// The number of items per page
    /// </summary>
    public int PageSize { get; set; }

}
