using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

public class DeleteProductProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteProduct operation.
    /// </summary>
    public DeleteProductProfile()
    {
          CreateMap<DeleteProductCommand, Product>();
          CreateMap<Product, DeleteProductResult>()
              .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
              .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => new RatingResult
              {
                  Rate = src.Rating.Rate,
                  Count = src.Rating.Count
              }));
    }
}
