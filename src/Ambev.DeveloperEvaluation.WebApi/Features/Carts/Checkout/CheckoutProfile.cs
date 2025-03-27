using Ambev.DeveloperEvaluation.Application.Carts.Checkout;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.Checkout;

public class CheckoutProfile : Profile
{
    public CheckoutProfile()
    {
        CreateMap<CheckoutRequest, CheckoutCommand>();
        CreateMap<CheckoutResult, CheckoutResponse>(); 
    }
}
