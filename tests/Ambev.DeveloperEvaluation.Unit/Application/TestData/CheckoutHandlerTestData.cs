using Ambev.DeveloperEvaluation.Application.Carts.Checkout;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CheckoutHandlerTestData
{
    private static readonly Faker<CheckoutCommand> checkoutHandlerFaker = new Faker<CheckoutCommand>()
        .RuleFor(c => c.CartId, f => f.Random.Int(1, 1000));

    public static CheckoutCommand GenerateValidCommand()
    {
        return checkoutHandlerFaker.Generate();
    }

    public static Cart GenerateValidCart(CheckoutCommand command)
    {
        return new Cart
        {
            Id = command.CartId,
            UserId = 123,
            UserName = "Test User",
            Date = System.DateTime.UtcNow,
            Status = CartStatus.Active,
            Items = GenerateCartItems(),
            TotalPrice = new Money(100)
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

    public static Sale GenerateValidSale(Cart cart)
    {
        var sale = new Sale(cart.UserId, cart.UserName);
        sale.AddItems(cart.Items.Select(cartItem =>
            new SaleItem(
                cartItem.ProductId,
                cartItem.ProductName,
                cartItem.Quantity,
                cartItem.UnitPrice,
                cartItem.Discount,
                cartItem.Total
            )).ToList());

        sale.TotalValue = cart.TotalPrice;
        return sale;
    }
}
