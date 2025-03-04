using Ambev.DeveloperEvaluation.Application.Products.ListCategories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListCategories;

public class ListCategoriesProfile : Profile
{
    public ListCategoriesProfile()
    {
        CreateMap<ListCategoriesRequest, ListCategoriesCommand>()
         .ReverseMap();
    }
}