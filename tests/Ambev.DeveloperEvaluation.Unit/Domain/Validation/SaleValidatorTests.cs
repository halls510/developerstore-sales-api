using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the SaleValidator class.
/// </summary>
public class SaleValidatorTests
{
    private readonly SaleValidator _validator;

    public SaleValidatorTests()
    {
        _validator = new SaleValidator();
    }

    [Fact(DisplayName = "Valid sale should pass validation")]
    public void Given_ValidSale_When_Validated_Then_ShouldNotHaveErrors()
    {
        var sale = SaleTestData.GenerateValidSale();
        var result = _validator.TestValidate(sale);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Sale with empty items should fail validation")]
    public void Given_EmptySaleItems_When_Validated_Then_ShouldHaveError()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Items.Clear();
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.Items).WithErrorMessage("Sale must contain at least one item.");
    }

    [Theory(DisplayName = "Invalid CustomerId should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_InvalidCustomerId_When_Validated_Then_ShouldHaveError(int customerId)
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.CustomerId = customerId;
        var result = _validator.TestValidate(sale);
        result.ShouldHaveValidationErrorFor(s => s.CustomerId).WithErrorMessage("CustomerId must be greater than zero.");
    }
}