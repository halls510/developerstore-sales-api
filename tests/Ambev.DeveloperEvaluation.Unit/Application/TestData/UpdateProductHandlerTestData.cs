using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateProductHandlerTestData
{
    private static readonly Faker<UpdateProductCommand> updateProductHandlerFaker = new Faker<UpdateProductCommand>()
        .RuleFor(p => p.Id, f => f.Random.Int(1, 1000))
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => new Money(f.Random.Decimal(1, 500)))
        .RuleFor(p => p.Description, f => f.Lorem.Sentence())
        .RuleFor(p => p.CategoryName, f => f.Commerce.Department())
        .RuleFor(p => p.Image, f => $"{f.Internet.UrlWithPath()}." + f.PickRandom("jpg", "jpeg", "png", "gif", "bmp", "webp")); // URL válida de imagem

    public static UpdateProductCommand GenerateValidCommand()
    {
        return updateProductHandlerFaker.Generate();
    }

    public static Product GenerateValidProduct(UpdateProductCommand command)
    {
        return new Product
        {
            Id = command.Id,
            Title = command.Title ?? "Default Product",
            Price = command.Price ?? new Money(100),
            Description = command.Description ?? "Default Description",
            CategoryId = 1,
            Category = new Category { Id = 1, Name = command.CategoryName ?? "Default Category" },
            Image = command.Image ?? "https://example.com/default.jpg",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
