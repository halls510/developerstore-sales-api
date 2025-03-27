
using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

/// <summary>
/// Handler for processing UpdateProductCommand requests
/// </summary>
public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductHandler> _logger;

    public UpdateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper, ILogger<UpdateProductHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateProductCommand request
    /// </summary>
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product {ProductId} with new details", command.Id);

        var validator = new UpdateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validation failed for UpdateProductCommand: {Errors}", validationResult.Errors);
            throw new ValidationException(validationResult.Errors);
        }

        var existingProduct = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.Id);
            throw new ResourceNotFoundException("Product not found", $"Product with ID {command.Id} not found.");
        }

        // Check if category exists, if not create it
        var category = await _categoryRepository.GetByNameAsync(command.CategoryName, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category {CategoryName} does not exist. Cannot update product.", command.CategoryName);
            category = new Category { Name = command.CategoryName };
            category = await _categoryRepository.CreateAsync(category, cancellationToken);
        }

        // Atualiza os dados do produto
        _mapper.Map(command, existingProduct);
        existingProduct.CategoryId = category.Id;
        existingProduct.Category = category;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        var updatedProduct = await _productRepository.UpdateAsync(existingProduct, cancellationToken);
        _logger.LogInformation("Product {ProductId} updated successfully", command.Id);
        return _mapper.Map<UpdateProductResult>(updatedProduct);
    }
}