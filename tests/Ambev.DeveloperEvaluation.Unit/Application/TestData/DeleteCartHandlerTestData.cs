using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class DeleteCartHandlerTestData
{
    private static readonly Faker<DeleteCartCommand> deleteCartHandlerFaker = new Faker<DeleteCartCommand>()
        .CustomInstantiator(f => new DeleteCartCommand(f.Random.Int(1, 1000)));

    public static DeleteCartCommand GenerateValidCommand()
    {
        return deleteCartHandlerFaker.Generate();
    }

    public static Cart GenerateValidCart(DeleteCartCommand command)
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
