using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CreateCartHandlerTestData
{
    private static readonly Faker<CreateCartCommand> createCartHandlerFaker = new Faker<CreateCartCommand>()
        .RuleFor(c => c.UserId, f => f.Random.Int(1, 1000))
        .RuleFor(c => c.UserName, f => f.Name.FullName())
        .RuleFor(c => c.Date, f => f.Date.Past())
        .RuleFor(c => c.Status, f => f.PickRandom<CartStatus>())
        .RuleFor(c => c.Items, f => GenerateValidCartItems());

    public static CreateCartCommand GenerateValidCommand()
    {
        return createCartHandlerFaker.Generate();
    }

    public static User GenerateValidUser(int userId)
    {
        return new User
        {
            Id = userId,
            Firstname = "John",
            Lastname = "Doe",
            Email = "john.doe@example.com"
        };
    }

    public static List<Product> GenerateValidProducts(List<CartItem> items)
    {
        return items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = "Product " + i.ProductId,
            Price = new Money(10 * i.ProductId),
            Description = "Sample Description",
            CategoryId = 1,
            Category = new Category { Id = 1, Name = "Category 1" },
            Image = "https://example.com/product.jpg"
        }).ToList();
    }

    public static Cart GenerateValidCart(CreateCartCommand command, User user, List<Product> products)
    {
        return new Cart
        {
            Id = new Random().Next(1, 1000),
            UserId = user.Id,
            UserName = user.Firstname + " " + user.Lastname,
            Items = command.Items,
            TotalPrice = new Money(command.Items.Sum(i => i.Quantity * (10 * i.ProductId))),
            Status = command.Status,
            Date = command.Date
        };
    }

    private static List<CartItem> GenerateValidCartItems()
    {
        return new Faker<CartItem>()
            .RuleFor(i => i.ProductId, f => f.Random.Int(1, 10))
            .RuleFor(i => i.Quantity, f => f.Random.Int(1, 5))
            .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
            .RuleFor(i => i.UnitPrice, f => new Money(f.Random.Decimal(5, 100)))
            .RuleFor(i => i.Discount, f => new Money(f.Random.Decimal(0, 10)))
            .RuleFor(i => i.Total, (f, i) => new Money(i.Quantity * (i.UnitPrice.Amount - i.Discount.Amount)))
            .Generate(3);
    }
}