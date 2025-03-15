using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateProductHandlerTestData
{
    private static readonly Faker<CreateProductCommand> createProductHandlerFaker = new Faker<CreateProductCommand>()
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Image, f => $"{f.Internet.UrlWithPath()}." + f.PickRandom("jpg", "jpeg", "png", "gif", "bmp", "webp")) // URL válida de imagem
        .RuleFor(p => p.CategoryName, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.Description, f => f.Lorem.Sentence())
        .RuleFor(p => p.Price, f => new Money(f.Random.Decimal(1, 1000)));

    public static CreateProductCommand GenerateValidCommand()
    {
        return createProductHandlerFaker.Generate();
    }

    public static Product GenerateValidProduct(CreateProductCommand command)
    {
        return new Product
        {
            Id = new Random().Next(1, 1000),
            Title = command.Title,
            Image = command.Image,
            Category = new Category { Name = command.CategoryName },
            Description = command.Description,
            Price = command.Price
        };
    }
}
