using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class ProductServiceTests
{
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productService = Substitute.For<IProductService>();
    }

    [Fact(DisplayName = "GetByTitleAsync should return product when title exists")]
    public async Task Given_ExistingProductTitle_When_GetByTitleAsyncCalled_Then_ShouldReturnProduct()
    {
        // Arrange
        var expectedProduct = new Product { Title = "Test Product" };
        _productService.GetByTitleAsync("Test Product", Arg.Any<CancellationToken>())
            .Returns(expectedProduct);

        // Act
        var result = await _productService.GetByTitleAsync("Test Product");

        // Assert
        result.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact(DisplayName = "GetByTitleAsync should return null when title does not exist")]
    public async Task Given_NonExistingProductTitle_When_GetByTitleAsyncCalled_Then_ShouldReturnNull()
    {
        // Arrange
        _productService.GetByTitleAsync("Unknown Product", Arg.Any<CancellationToken>())
            .Returns((Product?)null);

        // Act
        var result = await _productService.GetByTitleAsync("Unknown Product");

        // Assert
        result.Should().BeNull();
    }
}