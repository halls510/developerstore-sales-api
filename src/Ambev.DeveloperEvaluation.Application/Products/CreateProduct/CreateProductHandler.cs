using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

/// <summary>
/// Handler for processing CreateProductCommand requests
/// </summary>
public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CreateProductHandler
    /// </summary>
    /// <param name="productRepository">The product repository</param>
    /// <param name="categoryRepository">The category repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public CreateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateProductCommand request
    /// </summary>
    /// <param name="command">The CreateProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created product details</returns>
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingProduct = await _productRepository.GetByTitleAsync(command.Title, cancellationToken);
        if (existingProduct != null)
            throw new BusinessRuleException($"Product with title '{command.Title}' already exists");

        // Check if category exists, if not create it
        var category = await _categoryRepository.GetByNameAsync(command.CategoryName, cancellationToken);
        if (category == null)
        {
            category = new Category { Name = command.CategoryName };
            category = await _categoryRepository.CreateAsync(category, cancellationToken);
        }

        var product = _mapper.Map<Product>(command);
        product.CategoryId = category.Id;
        product.Category = category;

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);
        return _mapper.Map<CreateProductResult>(createdProduct);
    }
}
