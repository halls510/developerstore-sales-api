using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Uploads;

public class UploadImageProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UploadImage operation
    /// </summary>
    public UploadImageProfile()
    {      
        CreateMap<string, UploadImageResult>()
            .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src));
    }
}
