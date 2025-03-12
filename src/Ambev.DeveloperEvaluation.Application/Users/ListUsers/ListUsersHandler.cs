using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

/// <summary>
/// Handler for listing users
/// </summary>
public class ListUsersHandler : IRequestHandler<ListUsersCommand, ListUsersResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListUsersHandler> _logger;

    public ListUsersHandler(IUserRepository userRepository, IMapper mapper, ILogger<ListUsersHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ListUsersResult> Handle(ListUsersCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando listagem de usuários - Página {Page}, Tamanho {Size}, Ordenação {OrderBy}",
            command.Page, command.Size, command.OrderBy ?? "default");

        var validator = new ListUsersCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando ListUsersCommand");
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando usuários do banco de dados...");
        var users = await _userRepository.GetUsersAsync(command.Page, command.Size, command.OrderBy, command.Filters, cancellationToken);
        var totalUsers = await _userRepository.CountUsersAsync(command.Filters, cancellationToken);

        _logger.LogInformation("Listagem de usuários concluída com {TotalUsers} usuários encontrados", users.Count);

        return new ListUsersResult
        {
            Users = _mapper.Map<List<GetUserResult>>(users),
            TotalItems = totalUsers,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
