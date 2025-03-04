using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

public class ListProductsByCategoryProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListProductsByCategory operation
    /// </summary>
    public ListProductsByCategoryProfile()
    {        
         CreateMap<Product, GetProductResult>()
             .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
             .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingResult
             {
                 Rate = src.Rating.Rate,
                 Count = src.Rating.Count
             }));
    }
}
