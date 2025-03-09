using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// AutoMapper profile for Sale creation in the Web API layer.
/// </summary>
public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        // Mapping from request to command
        CreateMap<CreateSaleRequest, CreateSaleCommand>();

        // Mapping from SaleItemRequest to SaleItemDto
        CreateMap<SaleItemRequest, SaleItemDto>();

        // Mapping from command result to response
        CreateMap<CreateSaleResult, CreateSaleResponse>()
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue.Amount));

        // Mapping from SaleItemResult to SaleItemResponse
        CreateMap<SaleItemResult, SaleItemResponse>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount));
    }
}
