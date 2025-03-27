using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Users.DeleteUser;

public class DeleteUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteUser operation.
    /// </summary>
    public DeleteUserProfile()
    {
        CreateMap<DeleteUserCommand, User>();
        CreateMap<User, DeleteUserResult>()
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
