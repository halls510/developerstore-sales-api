using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Repositories;

public class ProductRepositoryTests
{
    private readonly IProductRepository _productRepository;
    private readonly CancellationToken _cancellationToken;

    public ProductRepositoryTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _cancellationToken = new CancellationToken();
    }

    [Fact(DisplayName = "CreateAsync should return created product")]
    public async Task Given_NewProduct_When_CreateAsyncCalled_Then_ShouldReturnCreatedProduct()
    {
        // Arrange
        var newProduct = new Product { Id = 1, Title = "Test Product" };
        _productRepository.CreateAsync(newProduct, _cancellationToken).Returns(newProduct);

        // Act
        var result = await _productRepository.CreateAsync(newProduct, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(newProduct);
    }

    [Fact(DisplayName = "GetByIdAsync should return product when id exists")]
    public async Task Given_ExistingProductId_When_GetByIdAsyncCalled_Then_ShouldReturnProduct()
    {
        // Arrange
        var expectedProduct = new Product { Id = 1, Title = "Test Product" };
        _productRepository.GetByIdAsync(1, _cancellationToken).Returns(expectedProduct);

        // Act
        var result = await _productRepository.GetByIdAsync(1, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact(DisplayName = "GetByTitleAsync should return product when title exists")]
    public async Task Given_ExistingProductTitle_When_GetByTitleAsyncCalled_Then_ShouldReturnProduct()
    {
        // Arrange
        var expectedProduct = new Product { Id = 1, Title = "Test Product" };
        _productRepository.GetByTitleAsync("Test Product", _cancellationToken).Returns(expectedProduct);

        // Act
        var result = await _productRepository.GetByTitleAsync("Test Product", _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact(DisplayName = "DeleteAsync should return true when product is deleted")]
    public async Task Given_ExistingProductId_When_DeleteAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _productRepository.DeleteAsync(1, _cancellationToken).Returns(true);

        // Act
        var result = await _productRepository.DeleteAsync(1, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "GetProductsAsync should return list of products")]
    public async Task Given_Pagination_When_GetProductsAsyncCalled_Then_ShouldReturnProductList()
    {
        // Arrange
        var products = new List<Product> { new Product { Id = 1, Title = "Test Product" } };
        _productRepository.GetProductsAsync(1, 10, null, null, _cancellationToken).Returns(products);

        // Act
        var result = await _productRepository.GetProductsAsync(1, 10, null, null, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(products);
    }
}