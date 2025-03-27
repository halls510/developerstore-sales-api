using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker Faker = new Faker();

    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(f.Random.Int(1, 1000), f.Name.FullName()))
        .RuleFor(s => s.Items, f => SaleItemTestData.GenerateValidSaleItems(3));

    /// <summary>
    /// Gera uma venda válida com itens e valores calculados corretamente.
    /// </summary>
    public static Sale GenerateValidSale()
    {
        var customerId = Faker.Random.Int(1, 1000);
        var customerName = Faker.Name.FullName();
        var sale = new Sale(customerId, customerName);

        var saleItems = SaleItemTestData.GenerateValidSaleItems(3);
        sale.AddItems(saleItems);
        sale.RecalculateTotal();

        return sale;
    }

    /// <summary>
    /// Gera uma venda inválida com erros como cliente inválido e sem itens.
    /// </summary>
    public static Sale GenerateInvalidSale()
    {
        var sale = new Sale(0, ""); // CustomerId inválido e nome vazio

        // Não adiciona itens para simular um erro de venda vazia
        sale.RecalculateTotal(); // Total inválido (zero)

        return sale;
    }
}
