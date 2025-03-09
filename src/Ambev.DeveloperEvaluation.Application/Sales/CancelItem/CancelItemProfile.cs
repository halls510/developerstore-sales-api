using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;

public class CancelItemProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CancelItem operation.
    /// </summary>
    public CancelItemProfile()
    {
        // Mapping from CancelItemCommand to SaleItem
        CreateMap<CancelItemCommand, SaleItem>();

        // Mapping from SaleItem to CancelItemResult
        CreateMap<SaleItem, CancelItemResult>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
    }
}
