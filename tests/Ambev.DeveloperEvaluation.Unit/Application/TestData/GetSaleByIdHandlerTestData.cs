using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class GetSaleByIdHandlerTestData
{
    private static readonly Faker<GetSaleByIdQuery> getSaleByIdHandlerFaker = new Faker<GetSaleByIdQuery>()
        .RuleFor(c => c.Id, f => f.Random.Int(1, 1000));

    public static GetSaleByIdQuery GenerateValidQuery()
    {
        return getSaleByIdHandlerFaker.Generate();
    }

    public static Sale GenerateValidSale(GetSaleByIdQuery query)
    {
        var sale = new Sale(
            id: query.Id,
            customerId: 1001, // ID fictício do cliente
            customerName: "Test User",
            saleDate: DateTime.UtcNow.AddDays(-1), // Data fictícia (um dia atrás)
            status: SaleStatus.Pending // Status inicial como Pendente
        );

        // Adiciona itens fictícios à venda
        sale.AddItems(GenerateSaleItems(query.Id));

        // Recalcula o total da venda com base nos itens adicionados
        sale.RecalculateTotal();

        return sale;
    }

    private static List<SaleItem> GenerateSaleItems(int saleId)
    {
        return new List<SaleItem>
        {
            new SaleItem(1,saleId,1, "Product A", 2, new Money(10), new Money(2), new Money(8)),
            new SaleItem(2,saleId,2, "Product B", 1, new Money(15), new Money(3), new Money(12))
        };
    }
}
