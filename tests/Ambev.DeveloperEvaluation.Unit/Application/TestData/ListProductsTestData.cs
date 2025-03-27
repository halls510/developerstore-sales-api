using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListProductsHandlerTestData
{
    private static readonly Faker<ListProductsCommand> listProductsHandlerFaker = new Faker<ListProductsCommand>()
        .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
        .RuleFor(c => c.Size, f => f.Random.Int(5, 50))
        .RuleFor(c => c.OrderBy, f => f.Lorem.Word());

    public static ListProductsCommand GenerateValidCommand()
    {
        return listProductsHandlerFaker.Generate();
    }

    public static List<Product> GenerateProductsEntityList()
    {
        return new List<Product>
            {
                new Product { Id = 1, Title = "Product A", Price = new Money(10), Description = "Desc A", CategoryId = 1, Image = "imgA.jpg" },
                new Product { Id = 2, Title = "Product B", Price = new Money(15), Description = "Desc B", CategoryId = 2, Image = "imgB.jpg" }
            };
    }

    public static List<GetProductResult> GenerateProductsList()
    {
        return new List<GetProductResult>
            {
                new GetProductResult { Id = 1, Title = "Product A", Price = new Money(10), Description = "Desc A", Category = "Cat A", Image = "imgA.jpg" },
                new GetProductResult { Id = 2, Title = "Product B", Price = new Money(15), Description = "Desc B", Category = "Cat B", Image = "imgB.jpg" }
            };
    }
}