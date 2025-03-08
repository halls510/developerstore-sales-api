using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;

/// <summary>
/// Profile for mapping DeleteUser feature requests to commands
/// </summary>
public class DeleteUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteUser feature
    /// </summary>
    public DeleteUserProfile()
    {
        CreateMap<int, Application.Users.DeleteUser.DeleteUserCommand>()
            .ConstructUsing(id => new Application.Users.DeleteUser.DeleteUserCommand(id));

        CreateMap<DeleteUserResult, DeleteUserResponse>()
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => new NameResponse
          {
              Firstname = src.Name.Firstname,
              Lastname = src.Name.Lastname
          }))
          .ReverseMap();
    }
}
