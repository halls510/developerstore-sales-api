using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListCartsHandlerTestData
{
    private static readonly Faker<ListCartsCommand> listCartsHandlerFaker = new Faker<ListCartsCommand>()
        .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
        .RuleFor(c => c.Size, f => f.Random.Int(5, 50))
        .RuleFor(c => c.OrderBy, f => f.Lorem.Word());

    public static ListCartsCommand GenerateValidCommand()
    {
        return listCartsHandlerFaker.Generate();
    }

    public static List<Cart> GenerateCartsEntityList()
    {
        return new List<Cart>
        {
            new Cart { Id = 1, UserId = 101, UserName = "User A", Status = CartStatus.Active, TotalPrice = new Money(100), Date = DateTime.UtcNow },
            new Cart { Id = 2, UserId = 102, UserName = "User B", Status = CartStatus.Completed, TotalPrice = new Money(150), Date = DateTime.UtcNow }
        };
    }

    public static List<GetCartResult> GenerateCartsList()
    {
        return new List<GetCartResult>
        {
            new GetCartResult { Id = 1, UserId = 101, Status = CartStatus.Active },
            new GetCartResult { Id = 2, UserId = 102, Status = CartStatus.Completed }
        };
    }
}
