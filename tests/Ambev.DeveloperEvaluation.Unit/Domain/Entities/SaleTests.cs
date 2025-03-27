using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "Sale should be valid with correct data")]
    public void Given_ValidSale_When_Validated_Then_ShouldBeValid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var validator = new SaleValidator();

        // Act
        var result = validator.Validate(sale);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Sale should be invalid with missing customer information")]
    public void Given_SaleWithoutCustomer_When_Validated_Then_ShouldBeInvalid()
    {
        // Arrange
        var sale = SaleTestData.GenerateInvalidSale();
        var validator = new SaleValidator();

        // Act
        var result = validator.Validate(sale);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Sale should be cancelled correctly")]
    public void Given_ValidSale_When_Cancelled_Then_ShouldBeCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        sale.Status.Should().Be(SaleStatus.Cancelled);
        sale.Items.All(item => item.Status == SaleItemStatus.Cancelled).Should().BeTrue();
    }

    [Fact(DisplayName = "Cannot cancel a completed sale")]
    public void Given_CompletedSale_When_Cancelled_Then_ShouldThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.CompleteSale(); // Método adequado para marcar como Completed

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Cancel());
        Assert.Equal("Cannot cancel a completed sale.", exception.Message);
    }

}