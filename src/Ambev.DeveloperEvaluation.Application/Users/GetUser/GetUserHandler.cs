using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Users.GetUser;

/// <summary>
/// Handler for processing GetUserCommand requests
/// </summary>
public class GetUserHandler : IRequestHandler<GetUserCommand, GetUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The ILogger instance</param>
    public GetUserHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<GetUserHandler> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetUserCommand request
    /// </summary>
    /// <param name="request">The GetUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user details if found</returns>
    public async Task<GetUserResult> Handle(GetUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando busca do usu�rio {UserId}", request.Id);

        var validator = new GetUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na valida��o do comando GetUserCommand para o usu�rio {UserId}", request.Id);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando usu�rio {UserId} no banco de dados", request.Id);
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Usu�rio {UserId} n�o encontrado", request.Id);
            throw new ResourceNotFoundException("User not found", $"User with ID {request.Id} not found");
        }

        _logger.LogInformation("Usu�rio {UserId} encontrado com sucesso", request.Id);
        return _mapper.Map<GetUserResult>(user);
    }
}
