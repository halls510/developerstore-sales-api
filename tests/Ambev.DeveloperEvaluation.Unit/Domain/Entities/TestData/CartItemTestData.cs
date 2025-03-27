using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides test data for CartItem entity.
/// </summary>
public static class CartItemTestData
{
    private static readonly Faker _faker = new Faker();

    /// <summary>
    /// Generates a valid CartItem for testing.
    /// </summary>
    /// <returns>A valid CartItem object.</returns>
    public static CartItem GenerateValidCartItem()
    {
        return new CartItem
        {
            CartId = _faker.Random.Int(1, 1000),
            ProductId = _faker.Random.Int(1, 1000),
            ProductName = _faker.Commerce.ProductName(),
            UnitPrice = new Money(_faker.Random.Decimal(1, 1000)),
            Quantity = _faker.Random.Int(1, 10),
            Discount = new Money(_faker.Random.Decimal(0, 50)),
            Total = new Money(_faker.Random.Decimal(10, 2000))
        };
    }
}