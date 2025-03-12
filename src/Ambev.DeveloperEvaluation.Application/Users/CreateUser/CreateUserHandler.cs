using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.CreateUser;

/// <summary>
/// Handler for processing CreateUserCommand requests
/// </summary>
public class CreateUserHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<CreateUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="passwordHasher">The password hasher instance</param>
    /// <param name="logger">The logger instance</param>
    public CreateUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        ILogger<CreateUserHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateUserCommand request
    /// </summary>
    /// <param name="command">The CreateUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user details</returns>
    public async Task<CreateUserResult> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando criação de usuário com e-mail {Email}", command.Email);

        var validator = new CreateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando CreateUserCommand para o e-mail {Email}", command.Email);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Verificando se o e-mail {Email} já está em uso", command.Email);
        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingUser != null)
        {
            _logger.LogWarning("Usuário com e-mail {Email} já existe", command.Email);
            throw new BusinessRuleException($"User with email {command.Email} already exists");
        }

        _logger.LogInformation("Criando novo usuário com e-mail {Email}", command.Email);
        var user = _mapper.Map<User>(command);
        user.Password = _passwordHasher.HashPassword(command.Password);

        _logger.LogInformation("Salvando usuário no banco de dados");
        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        _logger.LogInformation("Usuário {UserId} criado com sucesso", createdUser.Id);
        return _mapper.Map<CreateUserResult>(createdUser);
    }
}
