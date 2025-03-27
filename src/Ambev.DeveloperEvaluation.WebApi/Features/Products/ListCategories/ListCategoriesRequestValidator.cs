using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListCategories;

/// <summary>
/// Validator for ListCategoriesRequest
/// </summary>
public class ListCategoriesRequestValidator : AbstractValidator<ListCategoriesRequest>
{
    public ListCategoriesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");
        RuleFor(x => x.Size).InclusiveBetween(1, 200).WithMessage("Size must be between 1 and 200");
        RuleFor(x => x.OrderBy).MaximumLength(50).WithMessage("OrderBy query too long");
    }
}