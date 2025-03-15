using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Ambev.DeveloperEvaluation.Application.Common.Messaging;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProduct;

/// <summary>
/// Handler for processing GetProductCommand requests
/// </summary>
public class GetProductHandler : IRequestHandler<GetProductCommand, GetProductResult>
{

    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductHandler> _logger;
    private readonly RabbitMqPublisher _rabbitMqPublisher;

    public GetProductHandler(
        IServiceProvider serviceProvider,
        IProductRepository productRepository,
        IMapper mapper,
        RabbitMqPublisher rabbitMqPublisher,
        ILogger<GetProductHandler> logger)
    {        
        _productRepository = productRepository;
        _mapper = mapper;
        _rabbitMqPublisher = rabbitMqPublisher;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetProductCommand request
    /// </summary>
    /// <param name="request">The GetProduct command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user details if found</returns>
    public async Task<GetProductResult> Handle(GetProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching product with ID {ProductId}", request.Id);
        var validator = new GetProductValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            throw new ResourceNotFoundException("Product not found", $"Product with ID {request.Id} not found");
        }

        await _rabbitMqPublisher.PublishAsync("Pesquisa no Product");

        _logger.LogInformation("Product with ID {ProductId} retrieved successfully", request.Id);
        return _mapper.Map<GetProductResult>(product);
    }
}
