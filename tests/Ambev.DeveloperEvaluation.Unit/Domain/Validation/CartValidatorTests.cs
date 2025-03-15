using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the CartValidator class.
/// </summary>
public class CartValidatorTests
{
    private readonly CartValidator _validator;

    public CartValidatorTests()
    {
        _validator = new CartValidator();
    }

    [Fact(DisplayName = "Valid cart should pass validation")]
    public void Given_ValidCart_When_Validated_Then_ShouldNotHaveErrors()
    {
        var cart = CartTestData.GenerateValidCart();
        var result = _validator.TestValidate(cart);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Cart with empty items should fail validation")]
    public void Given_EmptyCartItems_When_Validated_Then_ShouldHaveError()
    {
        var cart = CartTestData.GenerateValidCart();
        cart.Items.Clear();
        var result = _validator.TestValidate(cart);
        result.ShouldHaveValidationErrorFor(c => c.Items).WithErrorMessage("Cart must contain at least one item.");
    }

    [Theory(DisplayName = "Invalid UserId should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidUserId_When_Validated_Then_ShouldHaveError(int userId)
    {
        var cart = CartTestData.GenerateValidCart();
        cart.UserId = userId;
        var result = _validator.TestValidate(cart);
        result.ShouldHaveValidationErrorFor(c => c.UserId).WithErrorMessage("UserId must be greater than zero.");
    }
}