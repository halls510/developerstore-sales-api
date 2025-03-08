using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListCarts operation
    /// </summary>
    public ListCartsProfile()
    {
        CreateMap<Cart, GetCartResult>()
           .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items));

        CreateMap<CartItem, CartItemResult>();
       
    }
}
