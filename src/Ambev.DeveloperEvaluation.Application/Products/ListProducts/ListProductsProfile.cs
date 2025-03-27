using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListProducts operation
    /// </summary>
    public ListProductsProfile()
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
