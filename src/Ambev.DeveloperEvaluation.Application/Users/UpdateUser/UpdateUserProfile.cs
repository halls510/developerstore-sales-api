using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Profile for mapping between User entity and UpdateUserResponse.
/// </summary>
public class UpdateUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateUser operation.
    /// </summary>
    public UpdateUserProfile()
    {
        CreateMap<UpdateUserCommand, User>();
        CreateMap<User, UpdateUserResult>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameResult
            {
                Firstname = src.Firstname,
                Lastname = src.Lastname
            }))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<Address, AddressResult>()
            .ForMember(dest => dest.Geolocation, opt => opt.MapFrom(src => src.Geolocation));

        CreateMap<Geolocation, GeoLocationResult>();
    }
}
