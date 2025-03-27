using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the CartItemValidator class.
/// </summary>
public class CartItemValidatorTests
{
    private readonly CartItemValidator _validator;

    public CartItemValidatorTests()
    {
        _validator = new CartItemValidator();
    }

    [Fact(DisplayName = "Valid cart item should pass validation")]
    public void Given_ValidCartItem_When_Validated_Then_ShouldNotHaveErrors()
    {
        var item = CartItemTestData.GenerateValidCartItem();
        var result = _validator.TestValidate(item);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid ProductId should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidProductId_When_Validated_Then_ShouldHaveError(int productId)
    {
        var item = CartItemTestData.GenerateValidCartItem();
        item.ProductId = productId;
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(i => i.ProductId).WithErrorMessage("ProductId must be greater than zero.");
    }

    [Fact(DisplayName = "Cart item with negative discount should fail validation")]
    public void Given_NegativeDiscount_When_Assigned_Then_ShouldThrowException()
    {
        // Arrange
        var item = CartItemTestData.GenerateValidCartItem();

        // Act
        Action act = () => item.Discount = new Money(-5);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Money amount cannot be negative.*");
    }
}