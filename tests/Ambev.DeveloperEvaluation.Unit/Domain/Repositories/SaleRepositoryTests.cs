using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Repositories;

public class SaleRepositoryTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly CancellationToken _cancellationToken;

    public SaleRepositoryTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _cancellationToken = new CancellationToken();
    }

    [Fact(DisplayName = "ExistsAsync should return true when sale exists")]
    public async Task Given_ExistingSaleId_When_ExistsAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _saleRepository.ExistsAsync(1, _cancellationToken).Returns(true);

        // Act
        var result = await _saleRepository.ExistsAsync(1, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "CreateAsync should return created sale")]
    public async Task Given_NewSale_When_CreateAsyncCalled_Then_ShouldReturnCreatedSale()
    {
        // Arrange
        var newSale = new Sale { Id = 1, SaleNumber = "12345" };
        _saleRepository.CreateAsync(newSale, _cancellationToken).Returns(newSale);

        // Act
        var result = await _saleRepository.CreateAsync(newSale, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(newSale);
    }

    [Fact(DisplayName = "GetByIdAsync should return sale when id exists")]
    public async Task Given_ExistingSaleId_When_GetByIdAsyncCalled_Then_ShouldReturnSale()
    {
        // Arrange
        var expectedSale = new Sale { Id = 1, SaleNumber = "12345" };
        _saleRepository.GetByIdAsync(1, _cancellationToken).Returns(expectedSale);

        // Act
        var result = await _saleRepository.GetByIdAsync(1, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedSale);
    }

    [Fact(DisplayName = "DeleteAsync should return true when sale is deleted")]
    public async Task Given_ExistingSaleId_When_DeleteAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _saleRepository.DeleteAsync(1, _cancellationToken).Returns(true);

        // Act
        var result = await _saleRepository.DeleteAsync(1, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "GetSalesAsync should return list of sales")]
    public async Task Given_Pagination_When_GetSalesAsyncCalled_Then_ShouldReturnSaleList()
    {
        // Arrange
        var sales = new List<Sale> { new Sale { Id = 1, SaleNumber = "12345" } };
        _saleRepository.GetSalesAsync(1, 10, null, null, _cancellationToken).Returns(sales);

        // Act
        var result = await _saleRepository.GetSalesAsync(1, 10, null, null, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(sales);
    }
}