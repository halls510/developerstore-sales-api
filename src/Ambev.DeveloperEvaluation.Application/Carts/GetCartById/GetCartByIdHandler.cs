using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCartById;

/// <summary>
/// Handler for retrieving a cart by ID
/// </summary>
public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public GetCartByIdHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cart == null)
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");

        // Aplica a regra para verificar se pode ser recuperado
        OrderRules.CanCartBeRetrieved(cart.Status, throwException: true);

        return _mapper.Map<CartDto>(cart);
    }
}

