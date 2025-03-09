using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleProfile : Profile
{
    public CancelSaleProfile()
    {
        CreateMap<int, Application.Sales.CancelSale.CancelSaleCommand>()
        .ConstructUsing(id => new Application.Sales.CancelSale.CancelSaleCommand(id));

        CreateMap<CancelSaleResult, CancelSaleResponse>();
    }
}
