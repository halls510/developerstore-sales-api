using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

/// <summary>
/// Command for listing Products with filters
/// </summary>
public class ListProductsCommand : IRequest<ListProductsResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
}
