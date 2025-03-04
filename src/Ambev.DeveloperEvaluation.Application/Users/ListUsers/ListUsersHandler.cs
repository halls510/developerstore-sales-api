using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

/// <summary>
/// Handler for listing users
/// </summary>
public class ListUsersHandler : IRequestHandler<ListUsersCommand, ListUsersResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ListUsersHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ListUsersResult> Handle(ListUsersCommand command, CancellationToken cancellationToken)
    {       
        var validator = new ListUsersCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var users = await _userRepository.GetUsersAsync(command.Page, command.Size, command.OrderBy, cancellationToken);
        var totalUsers = await _userRepository.CountUsersAsync(cancellationToken);

        return new ListUsersResult
        {
            Users = _mapper.Map<List<GetUserResult>>(users),
            TotalItems = totalUsers,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
