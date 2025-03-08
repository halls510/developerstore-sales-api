namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

public class ListCategoriesResult
{
    /// <summary>
    /// The list of Categories
    /// </summary>
    public List<string> Categories { get; set; } = new List<string>();

    /// <summary>
    /// The total number of Cagtegories in the system
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
