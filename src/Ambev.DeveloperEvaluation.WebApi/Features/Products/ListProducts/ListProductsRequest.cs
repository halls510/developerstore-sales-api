using System.Text.Json.Serialization;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public class ListProductsRequest
{    
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;
    public string? OrderBy { get; set; }
    public Dictionary<string, string[]> Filters { get; set; } = new Dictionary<string, string[]>();

}

