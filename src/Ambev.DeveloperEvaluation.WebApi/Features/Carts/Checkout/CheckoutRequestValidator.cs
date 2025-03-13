using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.Checkout;

public class CheckoutRequestValidator : AbstractValidator<CheckoutRequest>
{
    /// <summary>
    /// Initializes validation rules for CheckoutRequest
    /// </summary>
    public CheckoutRequestValidator()
    {
        RuleFor(x => x.CartId)
            .NotEmpty()
            .WithMessage("Cart ID is required");
    }
}