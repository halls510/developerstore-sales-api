﻿using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCartById;

public class GetCartByIdProfile : Profile
{
    public GetCartByIdProfile()
    {
        // Mapeia a entidade Cart para CartDto
        CreateMap<Cart, CartDto>()
              .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.UserId))
              .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
              .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice.Amount));

        // Mapeia CartItem para CartItemDto
        CreateMap<CartItem, CartItemDto>();
    }
}
