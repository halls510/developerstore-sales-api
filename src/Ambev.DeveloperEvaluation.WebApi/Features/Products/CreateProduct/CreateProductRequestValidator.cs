using Ambev.DeveloperEvaluation.Domain.Validation;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

/// <summary>
/// Validator for CreateProductRequest that defines validation rules for product creation.
/// </summary>
/// <remarks>
/// Validation rules include:
/// - Title: Required, length between 3 and 200 characters
/// - Price: Must be a positive decimal
/// - Description: Required, maximum length of 1000 characters
/// - Category: Required, maximum length of 100 characters
/// - Image: Must be a valid URL format
/// - Rating Rate: Must be between 0 and 5
/// - Rating Count: Must be zero or greater
/// </remarks>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateProductRequestValidator with defined validation rules.
    /// </summary>
    public CreateProductRequestValidator()
    {
        RuleFor(product => product.Title)
            .NotEmpty()
            .Length(3, 200);

        RuleFor(product => product.Price)
            .GreaterThan(0);

        RuleFor(product => product.Description)
            .NotEmpty()
            .MaximumLength(1000);

        RuleFor(product => product.Category)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(product => product.Image).SetValidator(new UrlValidator());

        RuleFor(product => product.Rating.Rate)
            .InclusiveBetween(0, 5);

        RuleFor(product => product.Rating.Count)
            .GreaterThanOrEqualTo(0);
    }
}
