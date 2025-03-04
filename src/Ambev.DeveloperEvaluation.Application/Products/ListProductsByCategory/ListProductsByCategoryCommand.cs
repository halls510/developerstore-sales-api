using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

/// <summary>
/// Command for listing Products with filters
/// </summary>
public class ListProductsByCategoryCommand : IRequest<ListProductsByCategoryResult>
{
	public string CategoryName { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
}
