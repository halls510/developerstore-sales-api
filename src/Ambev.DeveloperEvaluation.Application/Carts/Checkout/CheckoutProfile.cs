using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

public class CheckoutProfile : Profile
{
    public CheckoutProfile()
    {
        CreateMap<Sale, CheckoutResult>()
           .ForMember(dest => dest.SaleId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
