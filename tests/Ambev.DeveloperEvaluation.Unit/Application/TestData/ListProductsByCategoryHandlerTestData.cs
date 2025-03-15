using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListProductsByCategoryHandlerTestData
{
    private static readonly Faker<ListProductsByCategoryCommand> listProductsByCategoryHandlerFaker = new Faker<ListProductsByCategoryCommand>()
        .RuleFor(c => c.CategoryName, f => f.Commerce.Department())
        .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
        .RuleFor(c => c.Size, f => f.Random.Int(5, 50))
        .RuleFor(c => c.OrderBy, f => f.Lorem.Word());

    public static ListProductsByCategoryCommand GenerateValidCommand()
    {
        return listProductsByCategoryHandlerFaker.Generate();
    }

    public static List<Product> GenerateProductsEntityList()
    {
        return new List<Product>
            {
                new Product { Id = 1, Title = "Product A", Price = new Money(10), Description = "Desc A", CategoryId = 1, Image = "imgA.jpg" },
                new Product { Id = 2, Title = "Product B", Price = new Money(15), Description = "Desc B", CategoryId = 1, Image = "imgB.jpg" }
            };
    }

    public static List<GetProductResult> GenerateProductsList()
    {
        return new List<GetProductResult>
            {
                new GetProductResult { Id = 1, Title = "Product A", Price = new Money(10), Description = "Desc A", Category = "Category A", Image = "imgA.jpg" },
                new GetProductResult { Id = 2, Title = "Product B", Price = new Money(15), Description = "Desc B", Category = "Category A", Image = "imgB.jpg" }
            };
    }
}
