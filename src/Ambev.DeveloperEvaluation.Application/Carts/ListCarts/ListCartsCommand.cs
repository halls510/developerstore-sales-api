using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

/// <summary>
/// Command for listing Carts with filters
/// </summary>
public class ListCartsCommand : IRequest<ListCartsResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
    public Dictionary<string, string[]> Filters { get; set; } = new Dictionary<string, string[]>();
}
