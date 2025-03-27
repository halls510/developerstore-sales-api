using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

/// <summary>
/// Command for listing Categories with filters
/// </summary>
public class ListCategoriesCommand : IRequest<ListCategoriesResult>
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
}
