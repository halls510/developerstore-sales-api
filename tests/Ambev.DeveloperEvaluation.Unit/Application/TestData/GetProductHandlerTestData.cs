using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetProductHandlerTestData
{
    private static readonly Faker<GetProductCommand> _getProductFaker = new Faker<GetProductCommand>()
        .CustomInstantiator(f => new GetProductCommand(f.Random.Int(1, 1000)));

    public static GetProductCommand GenerateValidCommand()
    {
        return _getProductFaker.Generate();
    }

    public static Product GenerateValidProduct(GetProductCommand command)
    {
        return new Product
        {
            Id = command.Id,
            Title = "TestProduct",
            Description = "A sample product for testing",
            Price = 99.99m
        };
    }
}
