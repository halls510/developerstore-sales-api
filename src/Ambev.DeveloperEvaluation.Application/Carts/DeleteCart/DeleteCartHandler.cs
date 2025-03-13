using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Handler for processing DeleteCartCommand requests
/// </summary>
public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, DeleteCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The ILogger instance</param>
    public DeleteCartHandler(
        ICartRepository cartRepository,
        IMapper mapper,
        ILogger<DeleteCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeleteCartCommand request
    /// </summary>
    /// <param name="request">The DeleteCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteCartResult> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando a remoção do carrinho {CartId}", request.Id);

        var validator = new DeleteCartValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando DeleteCartCommand para o carrinho {CartId}", request.Id);
            throw new ValidationException(validationResult.Errors);
        }

        var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Carrinho {CartId} não encontrado", request.Id);
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");
        }

        _logger.LogInformation("Validando se o carrinho {CartId} pode ser removido", request.Id);
        OrderRules.CanCartBeDeleted(cart.Status, throwException: true);

        _logger.LogInformation("Removendo o carrinho {CartId} do repositório", request.Id);
        var success = await _cartRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
        {
            _logger.LogWarning("Falha ao remover o carrinho {CartId}. Pode não existir.", request.Id);
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");
        }

        _logger.LogInformation("Carrinho {CartId} removido com sucesso", request.Id);
        return _mapper.Map<DeleteCartResult>(cart);
    }
}
