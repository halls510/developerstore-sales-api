using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleById;

/// <summary>
/// Profile do AutoMapper para mapear SaleDto para GetSaleByIdResponse.
/// </summary>
public class GetSaleByIdProfile : Profile
{
    public GetSaleByIdProfile()
    {
        CreateMap<SaleDto, GetSaleByIdResponse>()
           .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.SaleNumber, opt => opt.MapFrom(src => src.SaleNumber))
           .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
           .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
           .ForMember(dest => dest.SaleDate, opt => opt.MapFrom(src => src.SaleDate))
           .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue))
           .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => src.Branch))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
           .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<SaleItemDto, SaleItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
