using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProductsByCategory;

/// <summary>
/// Validator for ListProductsByCategoryRequest
/// </summary>
public class ListProductsByCategoryRequestValidator : AbstractValidator<ListProductsByCategoryRequest>
{
    public ListProductsByCategoryRequestValidator()
    {
    	RuleFor(x => x.CategoryName).NotEmpty().WithMessage("CategoryName is required").MaximumLength(100).WithMessage("CategoryName must be at most 100 characters long");
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100");
        RuleFor(x => x.OrderBy).MaximumLength(50).WithMessage("OrderBy query too long");
    }
}