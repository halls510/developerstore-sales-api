using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
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
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
               .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items ?? new List<CartItem>())); // Garante que a lista não seja nula

        CreateMap<CartItem, CartItemResult>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
           .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
           .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
           .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
           .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice ?? new Money(0))) // Garante que não retorne null
           .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount ?? new Money(0))) // Garante que não retorne null
           .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total ?? new Money(0))); // Garante que não retorne null    

    }
}
