using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for the Cart entity using the Bogus library.
/// </summary>
public static class CartTestData
{
    private static readonly Faker<CartItem> CartItemFaker = new Faker<CartItem>()
        .RuleFor(ci => ci.CartId, f => f.Random.Int(1, 1000))
        .RuleFor(ci => ci.ProductId, f => f.Random.Int(1, 1000))
        .RuleFor(ci => ci.ProductName, f => f.Commerce.ProductName())
        .RuleFor(ci => ci.UnitPrice, f => new Money(f.Random.Decimal(1, 500)))
        .RuleFor(ci => ci.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(ci => ci.Discount, f => new Money(f.Random.Decimal(0, 50)))
        .RuleFor(ci => ci.Total, (f, ci) => new Money((ci.UnitPrice.Amount * ci.Quantity) - ci.Discount.Amount));

    private static readonly Faker<Cart> CartFaker = new Faker<Cart>()
        .RuleFor(c => c.UserId, f => f.Random.Int(1, 1000))
        .RuleFor(c => c.UserName, f => f.Name.FullName())
        .RuleFor(c => c.Date, f => f.Date.Past())
        .RuleFor(c => c.Status, f => f.PickRandom<CartStatus>())
        .RuleFor(c => c.Items, f => CartItemFaker.Generate(f.Random.Int(1, 5)))
        .RuleFor(c => c.TotalPrice, (f, c) => new Money(c.Items.Sum(i => i.Total.Amount)));

    public static Cart GenerateValidCart() => CartFaker.Generate();

    public static Cart GenerateInvalidCart() => new Cart
    {
        UserId = 0, // Inválido: UserId não pode ser zero ou negativo
        UserName = "", // Inválido: Nome não pode ser vazio
        Date = DateTime.UtcNow.AddDays(1), // Inválido: Data no futuro
        Status = (CartStatus)99, // Inválido: Status inexistente
        Items = new List<CartItem>(), // Inválido: Carrinho vazio
        TotalPrice = new Money(-10) // Inválido: Total negativo
    };
}