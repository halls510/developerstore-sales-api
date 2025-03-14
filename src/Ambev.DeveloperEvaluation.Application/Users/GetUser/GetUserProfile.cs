using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUser;

/// <summary>
/// Profile for mapping between User entity and GetUserResponse
/// </summary>
public class GetUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetUser operation
    /// </summary>
    public GetUserProfile()
    {        
        CreateMap<User, UserDto>()
           .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameResult
           {
               Firstname = src.Firstname,
               Lastname = src.Lastname
           }))
           .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<Address, AddressResult>()
            .ForMember(dest => dest.Geolocation, opt => opt.MapFrom(src => src.Geolocation));

        CreateMap<Geolocation, GeoLocationResult>()
        .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => src.Lat.ToString("F4"))) // Formata latitude para string
        .ForMember(dest => dest.Long, opt => opt.MapFrom(src => src.Long.ToString("F4"))); // Formata longitude para string

    }
}
