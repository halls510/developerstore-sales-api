using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

public class DeleteCartProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteCart operation.
    /// </summary>
    public DeleteCartProfile()
    {
        CreateMap<Cart, DeleteCartResult>()
           .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items));

        CreateMap<CartItem, CartItemResult>();

        // Mapeia o CartId dentro dos CartItems
        CreateMap<DeleteCartCommand, Cart>();
    }
}
