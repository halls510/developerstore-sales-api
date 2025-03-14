using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class DeleteProductHandlerTestData
{
    private static readonly Faker<DeleteProductCommand> deleteProductHandlerFaker = new Faker<DeleteProductCommand>()
        .CustomInstantiator(f => new DeleteProductCommand(f.Random.Int(1, 1000)));

    public static DeleteProductCommand GenerateValidCommand()
    {
        return deleteProductHandlerFaker.Generate();
    }

    public static Product GenerateValidProduct(DeleteProductCommand command)
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

