using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

/// <summary>
/// Profile for mapping between Application and API UpdateProduct responses
/// </summary>
public class UpdateProductProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateProduct feature
    /// </summary>
    public UpdateProductProfile()
    {
        CreateMap<UpdateProductRequest, UpdateProductCommand>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new Rating
            {
                Rate = src.Rating.Rate,
                Count = src.Rating.Count
            }))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => new Money(src.Price)))
            .ReverseMap();

        CreateMap<UpdateProductResult, UpdateProductResponse>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingResponse
            {
                Rate = src.Rating.Rate,
                Count = src.Rating.Count
            }))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount));

        CreateMap<RatingRequest, Rating>();
        CreateMap<Rating, RatingResult>();
        CreateMap<RatingResult, RatingResponse>();
    }
}
