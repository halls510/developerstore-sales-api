using Ambev.DeveloperEvaluation.Application.Products.GetProduct;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsResult
{
    /// <summary>
    /// The list of Products
    /// </summary>
    public List<GetProductResult> Products { get; set; } = new List<GetProductResult>();

    /// <summary>
    /// The total number of Products in the system
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
