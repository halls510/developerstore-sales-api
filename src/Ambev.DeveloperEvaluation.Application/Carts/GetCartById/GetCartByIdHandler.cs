using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCartById;

/// <summary>
/// Handler for retrieving a cart by ID
/// </summary>
public class GetCartByIdHandler : IRequestHandler<GetCartByIdQuery, CartDto>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCartByIdHandler> _logger;

    public GetCartByIdHandler(ICartRepository cartRepository, IMapper mapper, ILogger<GetCartByIdHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CartDto> Handle(GetCartByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando busca do carrinho {CartId}", request.Id);

        var cart = await _cartRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Carrinho {CartId} não encontrado", request.Id);
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {request.Id} not found");
        }

        _logger.LogInformation("Validando se o carrinho {CartId} pode ser recuperado", request.Id);
        OrderRules.CanCartBeRetrieved(cart.Status, throwException: true);

        _logger.LogInformation("Carrinho {CartId} recuperado com sucesso", request.Id);
        return _mapper.Map<CartDto>(cart);
    }
}
