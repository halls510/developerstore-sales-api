using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

/// <summary>
/// Profile for mapping between User entity and CreateUserResponse
/// </summary>
public class CreateUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateUser operation
    /// </summary>
    public CreateUserProfile()
    {
        CreateMap<CreateUserCommand, User>();
        CreateMap<User, CreateUserResult>()
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
