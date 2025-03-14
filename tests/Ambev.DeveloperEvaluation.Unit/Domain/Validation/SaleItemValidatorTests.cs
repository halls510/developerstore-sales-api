using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the SaleItemValidator class.
/// </summary>
public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator;

    public SaleItemValidatorTests()
    {
        _validator = new SaleItemValidator();
    }

    [Fact(DisplayName = "Valid sale item should pass validation")]
    public void Given_ValidSaleItem_When_Validated_Then_ShouldNotHaveErrors()
    {
        var item = SaleItemTestData.GenerateValidSaleItem();
        var result = _validator.TestValidate(item);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid ProductId should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidProductId_When_Validated_Then_ShouldHaveError(int productId)
    {
        var item = SaleItemTestData.GenerateValidSaleItem();
        item.ProductId = productId;
        var result = _validator.TestValidate(item);
        result.ShouldHaveValidationErrorFor(i => i.ProductId).WithErrorMessage("ProductId must be greater than zero.");
    }

    [Fact(DisplayName = "Sale item with negative discount should fail validation")]
    public void Given_NegativeDiscount_When_Assigned_Then_ShouldThrowException()
    {
        // Arrange
        var item = SaleItemTestData.GenerateValidSaleItem();

        // Act
        Action act = () => item.Discount = new Money(-5);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Money amount cannot be negative.*");
    }
}