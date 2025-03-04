using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

public class ListCategoriesCommandValidator : AbstractValidator<ListCategoriesCommand>
{
    public ListCategoriesCommandValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");
        RuleFor(x => x.Size).InclusiveBetween(1, 200).WithMessage("Size must be between 1 and 200");
        RuleFor(x => x.OrderBy).MaximumLength(50).WithMessage("OrderBy query too long");
    }
}