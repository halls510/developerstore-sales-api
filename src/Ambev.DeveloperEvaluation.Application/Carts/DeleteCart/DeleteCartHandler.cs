using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;

namespace Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;

/// <summary>
/// Handler for processing DeleteCartCommand requests
/// </summary>
public class DeleteCartHandler : IRequestHandler<DeleteCartCommand, DeleteCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of DeleteCartHandler
    /// </summary>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public DeleteCartHandler(
        ICartRepository cartRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the DeleteCartCommand request
    /// </summary>
    /// <param name="request">The DeleteCart command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteCartResult> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteCartValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cart == null)
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");

        // Aplica a regra de negócio diretamente, lançando exceção se não puder ser deletado
        OrderRules.CanCartBeDeleted(cart.Status, throwException: true);

        var success = await _cartRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");
        
        return _mapper.Map<DeleteCartResult>(cart);
    }
}
