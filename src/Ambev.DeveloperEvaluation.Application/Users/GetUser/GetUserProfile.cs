using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;

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
        CreateMap<GetUserCommand, User>();
        CreateMap<User, GetUserResult>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameResult
            {
                Firstname = src.Firstname,
                Lastname = src.Lastname
            }))
            .ReverseMap();
    }
}
