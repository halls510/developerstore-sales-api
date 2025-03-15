using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateProductHandler>>();
        _handler = new UpdateProductHandler(_productRepository, _categoryRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid update request When updating product Then returns updated product result")]
    public async Task Handle_ValidRequest_ReturnsUpdatedProduct()
    {
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        var existingProduct = UpdateProductHandlerTestData.GenerateValidProduct(command);
        var expectedResult = new UpdateProductResult
        {
            Id = existingProduct.Id,
            Title = existingProduct.Title,
            Price = existingProduct.Price,
            Description = existingProduct.Description,
            Category = existingProduct.Category.Name,
            Image = existingProduct.Image
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingProduct);
        _categoryRepository.GetByNameAsync(command.CategoryName, Arg.Any<CancellationToken>()).Returns(existingProduct.Category);
        _productRepository.UpdateAsync(existingProduct, Arg.Any<CancellationToken>()).Returns(existingProduct);
        _mapper.Map<UpdateProductResult>(existingProduct).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(existingProduct.Id);
        result.Title.Should().Be(existingProduct.Title);
    }

    [Fact(DisplayName = "Given non-existing product When updating product Then throws ResourceNotFoundException")]
    public async Task Handle_ProductNotFound_ThrowsException()
    {
        var command = UpdateProductHandlerTestData.GenerateValidCommand();

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Product)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage($"Product with ID {command.Id} not found.");
    }
}
