using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

public class CheckoutProfile : Profile
{
    public CheckoutProfile()
    {      
        CreateMap<Sale, CheckoutResult>();
    }
}
