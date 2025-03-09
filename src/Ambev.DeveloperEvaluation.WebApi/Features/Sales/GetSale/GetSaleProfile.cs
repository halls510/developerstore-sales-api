using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping GetSale feature requests to commands
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale feature
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<int, GetSaleCommand>()
            .ConstructUsing(id => new GetSaleCommand(id));

        // Mapping from request to command
        CreateMap<GetSaleRequest, GetSaleCommand>();

        // Mapping from SaleItemRequest to SaleItemDto
        CreateMap<SaleItemRequest, SaleItemDto>();

        // Mapping from command result to response
        CreateMap<GetSaleResult, GetSaleResponse>()
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue.Amount));

        // Mapping from SaleItemResult to SaleItemResponse
        CreateMap<SaleItemResult, SaleItemResponse>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount));
    }
}
