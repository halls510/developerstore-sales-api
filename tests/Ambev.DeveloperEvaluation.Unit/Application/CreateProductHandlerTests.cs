using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateProductHandler>>();

        _handler = new CreateProductHandler(_productRepository, _categoryRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProduct()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByTitleAsync(command.Title, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Product>(null));
        _categoryRepository.GetByNameAsync(command.CategoryName, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Category { Id = 1, Name = command.CategoryName }));
        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()).Returns(callInfo => callInfo.Arg<Product>());
        _mapper.Map<Product>(command).Returns(new Product { Title = command.Title, CategoryId = 1 });
        _mapper.Map<CreateProductResult>(Arg.Any<Product>()).Returns(new CreateProductResult { Title = command.Title });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.Title, result.Title);
    }

    [Fact]
    public async Task Handle_DuplicateProduct_ShouldThrowException()
    {
        // Arrange
        var command = CreateProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByTitleAsync(command.Title, Arg.Any<CancellationToken>()).Returns(Task.FromResult(new Product { Title = command.Title }));

        // Act & Assert
        await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidCommand_ShouldThrowValidationException()
    {
        // Arrange
        var command = new CreateProductCommand { Title = "", CategoryName = "" }; // Invalid input
        var validator = new CreateProductCommandValidator();
        var validationResult = await validator.ValidateAsync(command);

        // Act & Assert
        Assert.False(validationResult.IsValid);
    }
}
