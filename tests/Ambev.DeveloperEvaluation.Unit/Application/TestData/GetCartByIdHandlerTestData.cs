using Ambev.DeveloperEvaluation.Application.Carts.GetCartById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetCartByIdHandlerTestData
{
    private static readonly Faker<GetCartByIdQuery> getCartByIdHandlerFaker = new Faker<GetCartByIdQuery>()
        .CustomInstantiator(f => new GetCartByIdQuery(f.Random.Int(1, 1000)));

    public static GetCartByIdQuery GenerateValidQuery()
    {
        return getCartByIdHandlerFaker.Generate();
    }

    public static Cart GenerateValidCart(GetCartByIdQuery query)
    {
        return new Cart
        {
            Id = query.Id,
            UserId = new Random().Next(1, 1000),
            UserName = "Test User",
            Items = new List<CartItem>(),
            TotalPrice = new Money(0),
            Status = CartStatus.Active,
            Date = DateTime.UtcNow
        };
    }
}
