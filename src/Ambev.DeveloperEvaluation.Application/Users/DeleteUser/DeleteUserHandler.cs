using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.DeleteUser;

/// <summary>
/// Handler for processing DeleteUserCommand requests
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The ILogger instance</param>
    public DeleteUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<DeleteUserHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeleteUserCommand request
    /// </summary>
    /// <param name="request">The DeleteUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteUserResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando remoção do usuário {UserId}", request.Id);

        var validator = new DeleteUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando DeleteUserCommand para o usuário {UserId}", request.Id);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando usuário {UserId} no banco de dados", request.Id);
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", request.Id);
            throw new ResourceNotFoundException("User not found", $"User with ID {request.Id} not found");
        }

        _logger.LogInformation("Removendo usuário {UserId} do banco de dados", request.Id);
        var success = await _userRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
        {
            _logger.LogWarning("Falha ao remover o usuário {UserId}. Pode não existir.", request.Id);
            throw new ResourceNotFoundException("User not found", $"User with ID {request.Id} not found");
        }

        _logger.LogInformation("Usuário {UserId} removido com sucesso", request.Id);
        return _mapper.Map<DeleteUserResult>(user);
    }
}
