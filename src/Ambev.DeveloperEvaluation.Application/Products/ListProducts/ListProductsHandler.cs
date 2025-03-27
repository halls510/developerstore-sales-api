using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

/// <summary>
/// Handler for listing Products
/// </summary>
public class ListProductsHandler : IRequestHandler<ListProductsCommand, ListProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListProductsHandler> _logger;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper, ILogger<ListProductsHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ListProductsResult> Handle(ListProductsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing products with Page: {Page}, Size: {Size}, OrderBy: {OrderBy}", command.Page, command.Size, command.OrderBy);

        var validator = new ListProductsCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for ListProductsCommand: {Errors}", validationResult.Errors);
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }
        var Products = await _productRepository.GetProductsAsync(command.Page, command.Size, command.OrderBy, command.Filters, cancellationToken);
        var totalProducts = await _productRepository.CountProductsAsync(command.Filters, cancellationToken);

        _logger.LogInformation("Successfully retrieved {TotalItems} products", totalProducts);

        return new ListProductsResult
        {
            Products = _mapper.Map<List<GetProductResult>>(Products),
            TotalItems = totalProducts,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
