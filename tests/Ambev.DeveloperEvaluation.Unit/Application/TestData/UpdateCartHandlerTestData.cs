using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateCartHandlerTestData
{
    private static readonly Faker<UpdateCartCommand> updateCartHandlerFaker = new Faker<UpdateCartCommand>()
        .RuleFor(c => c.Id, f => f.Random.Int(1, 1000))
        .RuleFor(c => c.UserId, f => f.Random.Int(1, 1000))
        .RuleFor(c => c.Date, f => f.Date.Past())
        .RuleFor(c => c.Items, f => GenerateCartItems());

    public static UpdateCartCommand GenerateValidCommand()
    {
        return updateCartHandlerFaker.Generate();
    }

    public static Cart GenerateValidCart(UpdateCartCommand command)
    {
        return new Cart
        {
            Id = command.Id,
            UserId = command.UserId,
            UserName = "Test User",
            Date = command.Date,
            Items = command.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ProductName = $"Product {i.ProductId}",
                UnitPrice = new Money(10),
                Discount = new Money(2),
                Total = new Money(8)
            }).ToList(),
            TotalPrice = new Money(command.Items.Sum(i => 8 * i.Quantity)),
            Status = CartStatus.Active
        };
    }

    private static List<CartItem> GenerateCartItems()
    {
        return new List<CartItem>
        {
            new CartItem { ProductId = 1, Quantity = 2, ProductName = "Product 1", UnitPrice = new Money(10), Discount = new Money(2), Total = new Money(8) },
            new CartItem { ProductId = 2, Quantity = 3, ProductName = "Product 2", UnitPrice = new Money(15), Discount = new Money(3), Total = new Money(12) }
        };
    }
}
