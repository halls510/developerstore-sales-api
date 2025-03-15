using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class ListSalesHandlerTestData
{
    private static readonly Faker<ListSalesCommand> listSalesHandlerFaker = new Faker<ListSalesCommand>()
        .RuleFor(c => c.Page, f => f.Random.Int(1, 10))
        .RuleFor(c => c.Size, f => f.Random.Int(5, 50))
        .RuleFor(c => c.OrderBy, f => f.Lorem.Word());

    public static ListSalesCommand GenerateValidCommand()
    {
        return listSalesHandlerFaker.Generate();
    }

    public static List<Sale> GenerateSalesEntityList()
    {
        var sales = new List<Sale>
        {
            new Sale(1, "Customer A")
            {
                Items = new List<SaleItem>
                {
                    new SaleItem(1, "Product A", 2, new Money(10), new Money(2), new Money(8))
                }
            },
            new Sale(2, "Customer B")
            {
                Items = new List<SaleItem>
                {
                    new SaleItem(2, "Product B", 1, new Money(15), new Money(3), new Money(12))
                }
            }
        };

        sales[1].CompleteSale();
        return sales;
    }

    public static List<GetSaleResult> GenerateSalesList()
    {
        return new List<GetSaleResult>
        {
            new GetSaleResult { SaleId = 1, CustomerName = "Customer A", Status = SaleStatus.Pending.ToString() },
            new GetSaleResult { SaleId = 2, CustomerName = "Customer B", Status = SaleStatus.Completed.ToString() }
        };
    }
}
