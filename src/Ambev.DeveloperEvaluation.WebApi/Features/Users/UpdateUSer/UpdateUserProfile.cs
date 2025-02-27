using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Profile for mapping between Application and API UpdateUser responses.
/// </summary>
public class UpdateUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateUser feature.
    /// </summary>
    public UpdateUserProfile()
    {
        CreateMap<UpdateUserRequest, UpdateUserCommand>()
            .ForMember(dest => dest.Firstname, opt => opt.MapFrom(src => src.Name.Firstname))
            .ForMember(dest => dest.Lastname, opt => opt.MapFrom(src => src.Name.Lastname))
            .ReverseMap();

        CreateMap<UpdateUserResult, UpdateUserResponse>();
    }
}
