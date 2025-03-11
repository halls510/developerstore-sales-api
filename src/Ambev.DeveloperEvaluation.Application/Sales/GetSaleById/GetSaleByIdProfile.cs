using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Common;

/// <summary>
/// Profile do AutoMapper para mapear entidades para DTOs.
/// </summary>
public class GetSaleByIdProfile : Profile
{
    public GetSaleByIdProfile()
    {
        // Mapeia a entidade Sale para SaleDto
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.TotalValue, opt => opt.MapFrom(src => src.TotalValue));

        // Mapeia SaleItem para SaleItemDto
        CreateMap<SaleItem, SaleItemDto>();
    }
}
