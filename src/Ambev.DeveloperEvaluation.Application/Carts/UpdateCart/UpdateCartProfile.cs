using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

/// <summary>
/// Profile for mapping between Cart entity and UpdateCartResponse
/// </summary>
public class UpdateCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateCart operation
    /// </summary>
    public UpdateCartProfile()
    {
        CreateMap<Cart, UpdateCartResult>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items));

        CreateMap<CartItem, CartItemResult>();

        // Mapeia o CartId dentro dos CartItems
        CreateMap<UpdateCartCommand, Cart>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Date, DateTimeKind.Utc)));

    }
}
