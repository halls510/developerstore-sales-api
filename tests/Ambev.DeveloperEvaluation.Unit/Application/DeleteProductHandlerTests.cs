using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _cartRepository = Substitute.For<ICartRepository>();
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<DeleteProductHandler>>();
        _handler = new DeleteProductHandler(_productRepository, _cartRepository, _saleRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_Should_Delete_Product_When_Valid()
    {
        var command = DeleteProductHandlerTestData.GenerateValidCommand();
        var product = DeleteProductHandlerTestData.GenerateValidProduct(command);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);
        _cartRepository.IsProductInAnyCartAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);
        _saleRepository.IsProductInAnySaleAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);
        _productRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);
        _mapper.Map<DeleteProductResult>(product).Returns(new DeleteProductResult());

        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_Should_Throw_ResourceNotFoundException_When_Product_Not_Found()
    {
        // Arrange
        var command = DeleteProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Product>(null));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        Assert.Equal($"Product with ID {command.Id} not found", exception.Message);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Product_In_Cart()
    {
        var command = DeleteProductHandlerTestData.GenerateValidCommand();
        var product = DeleteProductHandlerTestData.GenerateValidProduct(command);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);
        _cartRepository.IsProductInAnyCartAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Product_In_Sale()
    {
        var command = DeleteProductHandlerTestData.GenerateValidCommand();
        var product = DeleteProductHandlerTestData.GenerateValidProduct(command);

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(product);
        _saleRepository.IsProductInAnySaleAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);

        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
