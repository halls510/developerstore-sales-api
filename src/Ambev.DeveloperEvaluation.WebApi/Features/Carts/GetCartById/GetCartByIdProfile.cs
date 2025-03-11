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
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items));

        // Mapeia CartItemDto para GetCartItemResponse
        CreateMap<CartItemDto, CartItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}
