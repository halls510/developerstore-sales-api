using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// AutoMapper profile for Sale updating in the Web API layer.
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleRequest, UpdateSaleCommand>();

        CreateMap<SaleItemRequest, SaleItemDto>();

        CreateMap<UpdateSaleResult, UpdateSaleResponse>();

        CreateMap<SaleItemResult, SaleItemResponse>()
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount.Amount))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount));
    }
}