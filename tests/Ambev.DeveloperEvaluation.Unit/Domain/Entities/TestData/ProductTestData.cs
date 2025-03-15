using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;


/// <summary>
/// Provides methods for generating test data for the Product entity using the Bogus library.
/// </summary>
public static class ProductTestData
{
    private static readonly Faker<Product> ProductFaker = new Faker<Product>()
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => new Money(f.Random.Decimal(1, 1000))) // Preço entre 1 e 1000
        .RuleFor(p => p.Description, f => f.Lorem.Paragraphs(1))
        .RuleFor(p => p.Image, f => $"{f.Internet.UrlWithPath()}." + f.PickRandom("jpg", "jpeg", "png", "gif", "bmp", "webp")) // URL válida de imagem
        .RuleFor(p => p.Rating, f => new Rating { Rate = f.Random.Double(0, 5), Count = f.Random.Int(0, 1000) })
        .RuleFor(p => p.CategoryId, f => f.Random.Int(1, 100)) // ID válido de categoria
        .RuleFor(p => p.Category, f => new Category { Id = f.Random.Int(1, 100), Name = f.Commerce.Department() })
        .RuleFor(p => p.CreatedAt, f => f.Date.Past())
        .RuleFor(p => p.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Generates a valid Product entity with randomized data.
    /// </summary>
    public static Product GenerateValidProduct() => ProductFaker.Generate();

    /// <summary>
    /// Generates an invalid Product entity with missing or incorrect values.
    /// </summary>
    public static Product GenerateInvalidProduct() => new Product
    {
        Title = "", // Inválido: título vazio
        Price = new Money(0), // Inválido: preço menor ou igual a zero
        Description = new string('A', 1001), // Inválido: descrição maior que 1000 caracteres
        Image = "invalid-url", // Inválido: URL não é válida
        Rating = new Rating { Rate = 6, Count = -1 }, // Inválido: Rating acima de 5 e count negativo
        CategoryId = 0, // Inválido: ID de categoria menor que 1
        Category = new Category { Id = 0, Name = "" } // Inválido: categoria sem nome
    };
}
