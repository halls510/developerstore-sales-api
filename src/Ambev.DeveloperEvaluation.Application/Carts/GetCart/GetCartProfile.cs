using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

/// <summary>
/// Profile for mapping between Product entity and GetCartResponse
/// </summary>
public class GetCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetCart operation
    /// </summary>
    public GetCartProfile()
    {

        CreateMap<Cart, GetCartResult>()
           .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items));

        CreateMap<CartItem, CartItemResult>();

        // Mapeia o CartId dentro dos CartItems
        CreateMap<GetCartCommand, Cart>();

    }
}
