using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

/// <summary>
/// Validator for ListCartsRequest
/// </summary>
public class ListCartsRequestValidator : AbstractValidator<ListCartsRequest>
{
    public ListCartsRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100");
        RuleFor(x => x.OrderBy).MaximumLength(50).WithMessage("OrderBy query too long");
    }
}