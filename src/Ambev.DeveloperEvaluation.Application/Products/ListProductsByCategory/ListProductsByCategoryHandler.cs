using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

/// <summary>
/// Handler for listing Products
/// </summary>
public class ListProductsByCategoryHandler : IRequestHandler<ListProductsByCategoryCommand, ListProductsByCategoryResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListProductsByCategoryHandler> _logger;

    public ListProductsByCategoryHandler(IProductRepository productRepository, IMapper mapper, ILogger<ListProductsByCategoryHandler> logger)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ListProductsByCategoryResult> Handle(ListProductsByCategoryCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Listing products for category {Category} with Page: {Page}, Size: {Size}, OrderBy: {OrderBy}",
           command.CategoryName, command.Page, command.Size, command.OrderBy);

        var validator = new ListProductsByCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for ListProductsByCategoryCommand: {Errors}", validationResult.Errors);
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var Products = await _productRepository.GetProductsByCategoryAsync(command.CategoryName, command.Page, command.Size, command.OrderBy, cancellationToken);
        var totalProducts = await _productRepository.CountProductsByCategoryAsync(command.CategoryName, cancellationToken);

        _logger.LogInformation("Successfully retrieved {TotalItems} products for category {Category}", totalProducts, command.CategoryName);

        return new ListProductsByCategoryResult
        {
            Products = _mapper.Map<List<GetProductResult>>(Products),
            TotalItems = totalProducts,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
