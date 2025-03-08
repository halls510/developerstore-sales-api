using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProductsByCategory;

public class ListProductsByCategoryRequest
{    
	public string CategoryName { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
}

