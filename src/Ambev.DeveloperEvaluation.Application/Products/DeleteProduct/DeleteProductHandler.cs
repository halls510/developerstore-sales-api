using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;

/// <summary>
/// Handler for processing DeleteProductCommand requests
/// </summary>
public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, DeleteProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteProductHandler> _logger;

    /// <summary>
    /// Initializes a new instance of DeleteProductHandler
    /// </summary>
    /// <param name="productRepository">The product repository</param>
    /// <param name="cartRepository">The cart repository</param>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The Logger instance</param>
    public DeleteProductHandler(
        IProductRepository productRepository,
        ICartRepository cartRepository,
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<DeleteProductHandler> logger)
    {
        _productRepository = productRepository;
        _cartRepository = cartRepository;
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the DeleteProductCommand request
    /// </summary>
    /// <param name="request">The DeleteProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteProductResult> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking if product {ProductId} can be deleted", request.Id);

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", request.Id);
            throw new ResourceNotFoundException("Product not found", $"Product with ID {request.Id} not found");
        }

        // Verifica se o produto está em um carrinho ativo
        var isInCart = await _cartRepository.IsProductInAnyCartAsync(request.Id, cancellationToken);
        if (isInCart)
        {
            _logger.LogWarning("Product {ProductId} cannot be deleted as it is in a cart", request.Id);
            throw new BusinessRuleException($"Cannot delete product {request.Id} as it is in an active cart");
        }

        // Verifica se o produto está em uma venda ativa
        var isInSale = await _saleRepository.IsProductInAnySaleAsync(request.Id, cancellationToken);
        if (isInSale)
        {
            _logger.LogWarning("Product {ProductId} cannot be deleted as it is in a sale", request.Id);
            throw new BusinessRuleException($"Cannot delete product {request.Id} as it is included in a sale");
        }

        var success = await _productRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
        {
            _logger.LogError("Failed to delete product {ProductId}", request.Id);
            throw new ResourceNotFoundException("Product deletion failed", $"Failed to delete product with ID {request.Id}");
        }

        _logger.LogInformation("Product {ProductId} deleted successfully", request.Id);
        return _mapper.Map<DeleteProductResult>(product);
    }
}
