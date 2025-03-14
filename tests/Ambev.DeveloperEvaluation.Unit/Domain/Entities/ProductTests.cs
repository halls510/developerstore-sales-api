using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;


/// <summary>
/// Contains unit tests for the Product entity.
/// Tests cover validation scenarios for the Product properties.
/// </summary>
public class ProductTests
{
    /// <summary>
    /// Tests that validation passes when all product properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid product data")]
    public void Given_ValidProductData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();

        // Act
        var result = product.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when product properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid product data")]
    public void Given_InvalidProductData_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var product = ProductTestData.GenerateInvalidProduct();

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that a product's title must not be empty and must have a valid length.
    /// </summary>
    [Theory(DisplayName = "Product title should be valid")]
    [InlineData("Valid Product Name", true)]
    [InlineData("", false)]
    [InlineData("A", false)]
    [InlineData("This is a very long title that should still be valid because it is within 200 characters", true)]    
    public void Given_ProductTitle_When_Validated_Then_ShouldBeCorrect(string title, bool expectedIsValid)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Title = title;

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    /// <summary>
    /// Tests that a product's price must be greater than zero.
    /// </summary>
    [Theory(DisplayName = "Product price must be greater than zero")]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void Given_ProductPrice_When_Validated_Then_ShouldBeCorrect(decimal price, bool expectedIsValid)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Price = new Money(price);

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    /// <summary>
    /// Tests that a product's image URL must be valid.
    /// </summary>
    [Theory(DisplayName = "Product image URL should be valid")]
    [InlineData("https://example.com/image.jpg", true)]
    [InlineData("http://example.com/image.png", true)]
    [InlineData("invalid-url", false)]
    [InlineData("ftp://invalid.com/image.gif", false)]
    [InlineData("", false)]
    public void Given_ProductImage_When_Validated_Then_ShouldBeCorrect(string imageUrl, bool expectedIsValid)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Image = imageUrl;

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    /// <summary>
    /// Tests that a product's rating must be between 0 and 5.
    /// </summary>
    [Theory(DisplayName = "Product rating should be between 0 and 5")]
    [InlineData(0, true)]
    [InlineData(2.5, true)]
    [InlineData(5, true)]
    [InlineData(-1, false)]
    [InlineData(5.1, false)]
    public void Given_ProductRating_When_Validated_Then_ShouldBeCorrect(double rating, bool expectedIsValid)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Rating.Rate = rating;

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    /// <summary>
    /// Tests that a product's rating count must be zero or greater.
    /// </summary>
    [Theory(DisplayName = "Product rating count must be zero or greater")]
    [InlineData(0, true)]
    [InlineData(10, true)]
    [InlineData(-5, false)]
    public void Given_ProductRatingCount_When_Validated_Then_ShouldBeCorrect(int ratingCount, bool expectedIsValid)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.Rating.Count = ratingCount;

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }

    /// <summary>
    /// Tests that the product category must be valid.
    /// </summary>
    [Theory(DisplayName = "Product category should be valid")]
    [InlineData(1, "Electronics", true)]
    [InlineData(0, "Invalid Category", false)]
    [InlineData(2, "", false)]
    public void Given_ProductCategory_When_Validated_Then_ShouldBeCorrect(int categoryId, string categoryName, bool expectedIsValid)
    {
        // Arrange
        var product = ProductTestData.GenerateValidProduct();
        product.CategoryId = categoryId;
        product.Category = new Category { Id = categoryId, Name = categoryName };

        // Act
        var result = product.Validate();

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }
}
