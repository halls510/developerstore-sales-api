using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

/// <summary>
/// Handler for processing UpdateProductCommand requests
/// </summary>
public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateProductHandler
    /// </summary>
    public UpdateProductHandler(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateProductCommand request
    /// </summary>
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingProduct = await _productRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product with ID {command.Id} not found.");

        // Check if category exists, if not create it
        var category = await _categoryRepository.GetByNameAsync(command.CategoryName, cancellationToken);
        if (category == null)
        {
            category = new Category { Name = command.CategoryName };
            category = await _categoryRepository.CreateAsync(category, cancellationToken);
        }

        // Atualiza os dados do produto
        _mapper.Map(command, existingProduct);
        existingProduct.CategoryId = category.Id;
        existingProduct.Category = category;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        var updatedProduct = await _productRepository.UpdateAsync(existingProduct, cancellationToken);
        return _mapper.Map<UpdateProductResult>(updatedProduct);
    }
}