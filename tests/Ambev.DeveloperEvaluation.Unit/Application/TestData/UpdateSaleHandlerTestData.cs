using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker<UpdateSaleCommand> updateSaleHandlerFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => f.Random.Int(1, 1000))
        .RuleFor(s => s.CustomerId, f => f.Random.Int(1, 1000))
        .RuleFor(s => s.Items, f => GenerateSaleItems());

    public static UpdateSaleCommand GenerateValidCommand()
    {
        return updateSaleHandlerFaker.Generate();
    }

    public static Sale GenerateValidSale(UpdateSaleCommand command)
    {
        return new Sale(command.CustomerId, "Test Customer")
        {
            Id = command.Id,
            Items = command.Items.Select(i => new SaleItem(                
                i.ProductId,
                i.ProductName,
                i.Quantity,
                new Money(i.UnitPrice),
                new Money(i.Discount),
                new Money(i.Total)
            )).ToList(),
            TotalValue = new Money(command.Items.Sum(i => i.Total))
        };
    }

    private static List<SaleItemDto> GenerateSaleItems()
    {
        return new List<SaleItemDto>
        {
            new SaleItemDto
            {
                SaleId = 1,
                ProductId = 1,
                ProductName = "Product A",
                Quantity = 2,
                UnitPrice = 10,
                Discount = 2,
                Total = 8,
                Status = SaleItemStatus.Active
            },
            new SaleItemDto
            {
                SaleId = 1,
                ProductId = 2,
                ProductName = "Product B",
                Quantity = 3,
                UnitPrice = 15,
                Discount = 3,
                Total = 12,
                Status = SaleItemStatus.Active
            }
        };
    }
}