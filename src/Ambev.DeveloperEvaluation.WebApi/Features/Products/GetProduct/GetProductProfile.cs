using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

/// <summary>
/// Profile for mapping GetProduct feature requests to commands
/// </summary>
public class GetProductProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetProduct feature
    /// </summary>
    public GetProductProfile()
    {
        CreateMap<int, Application.Products.GetProduct.GetProductCommand>()
            .ConstructUsing(id => new Application.Products.GetProduct.GetProductCommand(id));  

        CreateMap<GetProductResult, GetProductResponse>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingResponse
            {
                Rate = src.Rating.Rate,
                Count = src.Rating.Count
            }))
            .ReverseMap();

        CreateMap<RatingRequest, Rating>();
        CreateMap<Rating, RatingResult>();
        CreateMap<RatingResult, RatingResponse>();

    }
}
