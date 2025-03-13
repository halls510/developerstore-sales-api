using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

/// <summary>
/// Handler for processing UpdateUserCommand requests.
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<UpdateUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateUserHandler.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="passwordHasher">The password hasher instance.</param>
    /// <param name="logger">The ILogger instance.</param>
    public UpdateUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ILogger<UpdateUserHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateUserCommand request.
    /// </summary>
    /// <param name="command">The UpdateUser command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user details.</returns>
    public async Task<UpdateUserResult> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização do usuário {UserId}", command.Id);

        var validator = new UpdateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando UpdateUserCommand para o usuário {UserId}", command.Id);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando usuário {UserId} no banco de dados", command.Id);
        var user = await _userRepository.GetByIdAsync(command.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", command.Id);
            throw new ResourceNotFoundException("User not found", $"User with ID {command.Id} not found.");
        }

        // Atualizar os dados do usuário
        _logger.LogInformation("Atualizando informações do usuário {UserId}", command.Id);
        _mapper.Map(command, user);

        // Hash da nova senha, caso tenha sido alterada
        if (!string.IsNullOrEmpty(command.Password))
        {
            _logger.LogInformation("Atualizando senha do usuário {UserId}", command.Id);
            user.Password = _passwordHasher.HashPassword(command.Password);
        }

        // Marcar a data de atualização
        user.UpdatedAt = DateTime.UtcNow;

        _logger.LogInformation("Salvando usuário {UserId} atualizado no banco de dados", command.Id);
        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Usuário {UserId} atualizado com sucesso", command.Id);
        return _mapper.Map<UpdateUserResult>(updatedUser);
    }
}
