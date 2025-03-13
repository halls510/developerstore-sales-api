using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

public class CheckoutCommandValidator : AbstractValidator<CheckoutCommand>
{
    /// <summary>
    /// Initializes validation rules for CheckoutCommand
    /// </summary>
    public CheckoutCommandValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty()
            .WithMessage("Cart ID is required");
    }
}


