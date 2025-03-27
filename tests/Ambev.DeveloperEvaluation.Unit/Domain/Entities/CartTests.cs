using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class CartTests
{
    [Fact(DisplayName = "Cart validation should pass for a valid cart")]
    public void Given_ValidCart_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var cart = CartTestData.GenerateValidCart();
        var validator = new CartValidator();

        // Act
        var result = validator.Validate(cart);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact(DisplayName = "Cart validation should fail for an invalid cart")]
    public void Given_InvalidCart_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var cart = CartTestData.GenerateInvalidCart();
        var validator = new CartValidator();

        // Act
        var result = validator.Validate(cart);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "Cart should not be checked out if it is already checked out")]
    public void Given_CheckedOutCart_When_MarkedAsCheckedOut_Then_ShouldThrowException()
    {
        // Arrange
        var cart = CartTestData.GenerateValidCart();
        cart.Status = CartStatus.CheckedOut;

        // Act & Assert
        Assert.Throws<BusinessRuleException>(() => cart.MarkAsCheckedOut());
    }

    [Fact(DisplayName = "Cart should not be checked out if it is empty")]
    public void Given_EmptyCart_When_MarkedAsCheckedOut_Then_ShouldThrowException()
    {
        // Arrange
        var cart = CartTestData.GenerateValidCart();
        cart.Items.Clear();

        // Act & Assert
        Assert.Throws<BusinessRuleException>(() => cart.MarkAsCheckedOut());
    }
}