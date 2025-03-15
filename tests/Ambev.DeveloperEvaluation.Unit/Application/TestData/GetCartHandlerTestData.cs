using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetCartHandlerTestData
{
    private static readonly Faker<GetCartCommand> getCartHandlerFaker = new Faker<GetCartCommand>()
        .CustomInstantiator(f => new GetCartCommand(f.Random.Int(1, 1000)));

    public static GetCartCommand GenerateValidCommand()
    {
        return getCartHandlerFaker.Generate();
    }

    public static Cart GenerateValidCart(GetCartCommand command)
    {
        return new Cart
        {
            Id = command.Id,
            UserId = new Random().Next(1, 1000),
            UserName = "Test User",
            Items = new List<CartItem>(),
            TotalPrice = new Money(0),
            Status = CartStatus.Active,
            Date = DateTime.UtcNow
        };
    }
}

