using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListUsers operation
    /// </summary>
    public ListUsersProfile()
    {
        CreateMap<User, GetUserResult>()
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
