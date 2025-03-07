using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart
{
    /// <summary>
    /// Profile for mapping between Cart entity and CreateCartResult
    /// </summary>
    public class CreateCartProfile : Profile
    {
        /// <summary>
        /// Initializes the mappings for CreateCart operation
        /// </summary>
        public CreateCartProfile()
        {      

            CreateMap<Cart, CreateCartResult>()
                .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Items));

            CreateMap<CartItem, CartItemResult>();

            // Mapeia o CartId dentro dos CartItems
            CreateMap<CreateCartCommand, Cart>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Date, DateTimeKind.Utc)));
           
        }
    }
}
