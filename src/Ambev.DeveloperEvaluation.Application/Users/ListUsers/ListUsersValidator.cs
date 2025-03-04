using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersCommandValidator : AbstractValidator<ListUsersCommand>
{
    public ListUsersCommandValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1");
        RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Size must be between 1 and 100");
        RuleFor(x => x.OrderBy).MaximumLength(50).WithMessage("OrderBy query too long");
    }
}