using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    public int Id { get; set; }    
    public int CustomerId { get; }    
    public List<SaleItemDto> Items { get; }

    public UpdateSaleCommand() { }

    public UpdateSaleCommand(int customerId,  List<SaleItemDto> items)
    {       
        CustomerId = customerId;
        Items = items;
    }
}