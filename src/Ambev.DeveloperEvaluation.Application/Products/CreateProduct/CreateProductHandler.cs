using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductHandler> _logger;

    public CreateProductHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IMapper mapper,
        ILogger<CreateProductHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing product creation: {ProductTitle}", command.Title);

        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for product: {ProductTitle}", command.Title);
            throw new ValidationException(validationResult.Errors);
        }

        var existingProduct = await _productRepository.GetByTitleAsync(command.Title, cancellationToken);
        if (existingProduct != null)
        {
            _logger.LogWarning("Duplicate product attempted: {ProductTitle}", command.Title);
            throw new BusinessRuleException($"Product with title '{command.Title}' already exists");
        }

        var category = await _categoryRepository.GetByNameAsync(command.CategoryName, cancellationToken);
        if (category == null)
        {
            _logger.LogInformation("Creating new category: {CategoryName}", command.CategoryName);
            category = new Category { Name = command.CategoryName };
            category = await _categoryRepository.CreateAsync(category, cancellationToken);
        }

        var product = _mapper.Map<Product>(command);
        product.CategoryId = category.Id;
        product.Category = category;

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);

        _logger.LogInformation("Product created successfully: {ProductTitle}", command.Title);
        return _mapper.Map<CreateProductResult>(createdProduct);
    }
}
