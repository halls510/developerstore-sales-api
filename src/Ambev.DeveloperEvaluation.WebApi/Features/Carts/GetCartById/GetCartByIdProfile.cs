using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCartById;
using Ambev.DeveloperEvaluation.WebApi.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCartById;

/// <summary>
/// Profile do AutoMapper para mapear CartDto para GetCartByIdResponse.
/// </summary>
public class GetCartByIdProfile : Profile
{
    public GetCartByIdProfile()
    {
        // Mapeia CartDto para GetCartByIdResponse
        CreateMap<CartDto, GetCartByIdResponse>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId)) // Garante que CustomerId seja mapeado corretamente
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedAt.Date)) // Apenas a data, sem horário
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

        // Mapeia CartItemDto para GetCartItemResponse
        CreateMap<CartItemDto, CartItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}
