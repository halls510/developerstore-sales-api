using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

/// <summary>
/// Profile for mapping between API request and UpdateUser command.
/// </summary>
public class UpdateUserRequestProfile : Profile
{
    public UpdateUserRequestProfile()
    {
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
    }
}
