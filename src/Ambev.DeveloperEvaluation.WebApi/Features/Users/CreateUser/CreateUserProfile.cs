using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

/// <summary>
/// Profile for mapping between Application and API CreateUser responses
/// </summary>
public class CreateUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateUser feature
    /// </summary>
    public CreateUserProfile()
    {
        CreateMap<CreateUserRequest, CreateUserCommand>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Name.Firstname))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Name.Lastname))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ReverseMap();

        CreateMap<AddressRequest, Address>()
            .ForMember(dest => dest.Geolocation, opt => opt.MapFrom(src => src.Geolocation));

        CreateMap<GeoLocationRequest, Geolocation>();

        CreateMap<NameResult, NameResponse>();

        CreateMap<AddressResult, AddressResponse>()
            .ForMember(dest => dest.Geolocation, opt => opt.MapFrom(src => src.Geolocation));

        CreateMap<GeoLocationResult, GeoLocationResponse>();

        CreateMap<CreateUserResult, CreateUserResponse>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
    }
}
