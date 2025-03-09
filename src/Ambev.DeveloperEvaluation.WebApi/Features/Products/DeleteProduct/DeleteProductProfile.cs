using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.DeleteProduct;

/// <summary>
/// Profile for mapping DeleteProduct feature requests to commands
/// </summary>
public class DeleteProductProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteProduct feature
    /// </summary>
    public DeleteProductProfile()
    {
        CreateMap<int, Application.Products.DeleteProduct.DeleteProductCommand>()
            .ConstructUsing(id => new Application.Products.DeleteProduct.DeleteProductCommand(id));

        CreateMap<DeleteProductResult, DeleteProductResponse>()
          .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingResponse
          {
              Rate = src.Rating.Rate,
              Count = src.Rating.Count
          }))
          .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price.Amount))
          .ReverseMap();

        CreateMap<RatingRequest, Rating>();
        CreateMap<Rating, RatingResult>();
        CreateMap<RatingResult, RatingResponse>();
    }
}
