using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the ProductValidator class.
/// Tests cover various product validation scenarios including required fields, length, and numerical constraints.
/// </summary>
public class ProductValidatorTests
{
    private readonly ProductValidator _validator;

    public ProductValidatorTests()
    {
        _validator = new ProductValidator();
    }

    /// <summary>
    /// Tests that validation passes for a valid product.
    /// </summary>
    [Fact(DisplayName = "Valid product should pass validation")]
    public void Given_ValidProduct_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that validation fails when the title is empty.
    /// </summary>
    [Fact(DisplayName = "Empty title should fail validation")]
    public void Given_EmptyTitle_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Title = string.Empty;

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Title)
            .WithErrorMessage("Title is required.");
    }    

    /// <summary>
    /// Tests that validation fails when the product image URL is invalid.
    /// </summary>
    [Theory(DisplayName = "Invalid image URLs should fail validation")]
    [InlineData("invalid_url")]
    [InlineData("http://example.com/image.txt")]
    [InlineData("https://example.com/image")]
    public void Given_InvalidImageUrl_When_Validated_Then_ShouldHaveError(string imageUrl)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Image = imageUrl;

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Image)
            .WithErrorMessage("Image must be a valid URL pointing to an image file (jpg, png, etc.).");
    }

    /// <summary>
    /// Tests that validation fails when the rating is out of bounds.
    /// </summary>
    [Theory(DisplayName = "Invalid rating values should fail validation")]
    [InlineData(-1)]
    [InlineData(5.5)]
    public void Given_InvalidRating_When_Validated_Then_ShouldHaveError(double rating)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Rating.Rate = rating;

        // Act
        var result = _validator.TestValidate(product);

        // Assert
        result.ShouldHaveValidationErrorFor(p => p.Rating.Rate)
            .WithErrorMessage("Rating must be between 0 and 5.");
    }
}