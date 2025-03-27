using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;

public class CancelItemProfile : Profile
{
    public CancelItemProfile()
    {
        CreateMap<CancelItemRequest, CancelItemCommand>(); 

        CreateMap<CancelItemResult, CancelItemResponse>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount.Amount))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total.Amount))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
