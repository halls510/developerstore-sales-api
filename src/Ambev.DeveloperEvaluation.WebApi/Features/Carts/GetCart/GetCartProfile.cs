using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;

/// <summary>
/// Profile for mapping GetCart feature requests to commands
/// </summary>
public class GetCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetCart feature
    /// </summary>
    public GetCartProfile()
    {
        CreateMap<int, Application.Carts.GetCart.GetCartCommand>()
            .ConstructUsing(id => new Application.Carts.GetCart.GetCartCommand(id));

        // Mapeia GetCartResult para GetCartResponse (para retorno da API)
        CreateMap<GetCartResult, GetCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        // Mapeia CartItemResult para CartItemResponse (para retorno da API)
        CreateMap<CartItemResult, CartItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

    }
}
