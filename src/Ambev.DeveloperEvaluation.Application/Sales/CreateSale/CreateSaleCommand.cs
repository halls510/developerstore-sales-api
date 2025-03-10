using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommand : IRequest<CreateSaleResult>
{    
    public int CustomerId { get; }
    public List<SaleItemDto> Items { get; }

    public CreateSaleCommand() { }

    public CreateSaleCommand(int customerId, List<SaleItemDto> items)
    {
        CustomerId = customerId;
        Items = items;
    }
}