using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

/// <summary>
/// Validator for UpdateCartCommand that defines validation rules for product creation command.
/// </summary>
public class UpdateCartCommandValidator : AbstractValidator<UpdateCartCommand>
{
    /// <summary>
    /// Initializes a new instance of the UpdateCartCommandValidator with defined validation rules.
    /// </summary>
    public UpdateCartCommandValidator()
    {
        RuleFor(x => x.UserId)
           .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("The cart must contain at least one product.");
    }
}
