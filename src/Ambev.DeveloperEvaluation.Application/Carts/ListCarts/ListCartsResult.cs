using Ambev.DeveloperEvaluation.Application.Carts.GetCart;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsResult
{
    /// <summary>
    /// The list of Carts
    /// </summary>
    public List<GetCartResult> Carts { get; set; } = new List<GetCartResult>();

    /// <summary>
    /// The total number of Carts in the system
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
