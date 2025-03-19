using Ambev.DeveloperEvaluation.Application.Common.Messaging;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductHandler> _logger;
    private readonly GetProductHandler _handler;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;

    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetProductHandler>>();
        _rabbitMqPublisher = Substitute.For<IRabbitMqPublisher>();
        _handler = new GetProductHandler(
            Substitute.For<IServiceProvider>(),
            _productRepository,
            _mapper,
            _rabbitMqPublisher,
            _logger);
    }

    [Fact]
    public async Task Handle_ValidProductId_ReturnsProduct()
    {
        // Arrange
        var command = GetProductHandlerTestData.GenerateValidCommand();
        var product = GetProductHandlerTestData.GenerateValidProduct(command);
        var expectedResult = new GetProductResult { Id = product.Id, Title = product.Title, Price = product.Price };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(product));
        _mapper.Map<GetProductResult>(product).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.Title, result.Title);
        Assert.Equal(expectedResult.Price, result.Price);
    }

    [Fact]
    public async Task Handle_InvalidProductId_ThrowsResourceNotFoundException()
    {
        // Arrange
        var command = GetProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product>(null));

        // Act & Assert
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new GetProductCommand(0); // ID inválido

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
