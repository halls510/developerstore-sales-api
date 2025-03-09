using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// AutoMapper profile for mapping Sale entity to GetSaleResult and handling GetSaleCommand.
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale operation.
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<Sale, GetSaleResult>()
            .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<SaleItem, SaleItemResult>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<GetSaleCommand, Sale>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}
