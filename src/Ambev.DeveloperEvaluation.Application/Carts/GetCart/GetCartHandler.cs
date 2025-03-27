using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

/// <summary>
/// Handler for processing GetCartCommand requests
/// </summary>
public class GetCartHandler : IRequestHandler<GetCartCommand, GetCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The ILogger instance</param>
    public GetCartHandler(
        ICartRepository cartRepository,
        IMapper mapper,
        ILogger<GetCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetCartCommand request
    /// </summary>
    /// <param name="request">The GetCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user details if found</returns>
    public async Task<GetCartResult> Handle(GetCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando busca do carrinho {CartId}", request.Id);

        var validator = new GetCartValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando GetCartCommand para o carrinho {CartId}", request.Id);
            throw new ValidationException(validationResult.Errors);
        }

        var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Carrinho {CartId} não encontrado", request.Id);
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");
        }

        _logger.LogInformation("Validando se o carrinho {CartId} pode ser recuperado", request.Id);
        OrderRules.CanCartBeRetrieved(cart.Status, throwException: true);

        _logger.LogInformation("Carrinho {CartId} recuperado com sucesso", request.Id);
        return _mapper.Map<GetCartResult>(cart);
    }
}
