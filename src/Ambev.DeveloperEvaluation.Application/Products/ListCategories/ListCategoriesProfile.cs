using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

/// <summary>
/// Profile for mapping between Category entity and ListCategoriesResult
/// </summary>
public class ListCategoriesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListCategories operation
    /// </summary>
    public ListCategoriesProfile()
    {
        CreateMap<Category, string>().ConvertUsing(src => src.Name);
        CreateMap<ListCategoriesResult, List<string>>().ConvertUsing(src => src.Categories);
    }
}
