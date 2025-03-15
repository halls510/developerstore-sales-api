using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class CancelItemHandlerTestData
{
    private static readonly Faker<CancelItemCommand> cancelItemHandlerFaker = new Faker<CancelItemCommand>()
        .RuleFor(c => c.SaleId, f => f.Random.Int(1, 1000))
        .RuleFor(c => c.ProductId, f => f.Random.Int(1, 100));

    public static CancelItemCommand GenerateValidCommand()
    {
        return cancelItemHandlerFaker.Generate();
    }   
  
    public static Sale GenerateValidSale(CancelItemCommand command)
    {
        var sale = new Sale(
            id: command.SaleId,
            customerId: 1001, // ID fictício do cliente
            customerName: "Test User",
            saleDate: DateTime.UtcNow.AddDays(-1), // Data fictícia (um dia atrás)
            status: SaleStatus.Pending // Status inicial como Pendente
        );

        // Adiciona itens fictícios à venda
        sale.AddItems(GenerateSaleItems(command.SaleId, command.ProductId));

        // Recalcula o total da venda com base nos itens adicionados
        sale.RecalculateTotal();

        return sale;
    }

    /// <summary>
    /// Gera uma venda com um item já cancelado, simulando um cenário de erro esperado.
    /// </summary>
    public static Sale GenerateSaleWithCancelledItem(CancelItemCommand command)
    {
        var sale = new Sale(
            id: command.SaleId,
            customerId: 1001,
            customerName: "Test User",
            saleDate: DateTime.UtcNow.AddDays(-1),
            status: SaleStatus.Pending
        );

        // Gera os itens da venda
        var saleItems = GenerateSaleItems(command.SaleId, command.ProductId);

        // Marca um dos itens como cancelado
        var cancelledItem = saleItems.FirstOrDefault(i => i.ProductId == command.ProductId);
        if (cancelledItem != null)
        {
            cancelledItem.Cancel(); // Cancela o item
        }

        sale.AddItems(saleItems);
        sale.RecalculateTotal();

        return sale;
    }

    /// <summary>
    /// Gera uma lista de `SaleItem`, garantindo que um dos itens tenha o `ProductId` correto.
    /// </summary>
    private static List<SaleItem> GenerateSaleItems(int saleId, int productId)
    {
        return new List<SaleItem>
        {
            new SaleItem(1, saleId, productId, "Product A", 2, new Money(10), new Money(2), new Money(8)), // Produto com ID correto
            new SaleItem(2, saleId, productId + 1, "Product B", 1, new Money(15), new Money(3), new Money(12)) // Produto diferente
        };
    }
}