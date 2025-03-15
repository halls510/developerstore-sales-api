using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleItemTestData
{
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .CustomInstantiator(f => new SaleItem(
            f.Random.Int(1, 1000), // ProductId
            f.Commerce.ProductName(), // ProductName
            f.Random.Int(1, 20), // Quantity (entre 1 e 20)
            new Money(f.Random.Decimal(1, 100)), // UnitPrice (entre 1 e 100)
            new Money(f.Random.Decimal(0, 20)), // Discount (entre 0 e 20)
            new Money(f.Random.Decimal(1, 1000)) // Total (entre 1 e 1000)
        ));

    /// <summary>
    /// Gera uma única instância válida de SaleItem.
    /// </summary>
    public static SaleItem GenerateValidSaleItem() => SaleItemFaker.Generate();

    /// <summary>
    /// Gera uma lista de instâncias válidas de SaleItem.
    /// </summary>
    public static List<SaleItem> GenerateValidSaleItems(int count) => SaleItemFaker.Generate(count);

    /// <summary>
    /// Gera uma instância inválida de SaleItem para testes de validação.
    /// </summary>
    public static SaleItem GenerateInvalidSaleItem() => new SaleItem(
        0, // Invalid ProductId
        "", // Invalid ProductName
        0, // Invalid Quantity (zero)
        new Money(0), // Invalid UnitPrice (zero)
        new Money(-5), // Invalid Discount (negativo)
        new Money(-10) // Invalid Total (negativo)
    );
}
