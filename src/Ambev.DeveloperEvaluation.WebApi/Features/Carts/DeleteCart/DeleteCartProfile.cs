using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.DeleteCart;

/// <summary>
/// Profile for mapping DeleteCart feature requests to commands
/// </summary>
public class DeleteCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteCart feature
    /// </summary>
    public DeleteCartProfile()
    {
        CreateMap<int, Application.Carts.DeleteCart.DeleteCartCommand>()
            .ConstructUsing(id => new Application.Carts.DeleteCart.DeleteCartCommand(id));

        // Mapeia DeleteCartResult para DeleteCartResponse (para retorno da API)
        CreateMap<DeleteCartResult, DeleteCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        // Mapeia CartItemResult para CartItemResponse (para retorno da API)
        CreateMap<CartItemResult, CartItemResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
    }
}
