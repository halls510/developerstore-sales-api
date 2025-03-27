using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

/// <summary>
/// Validator for UpdateCartRequestValidator that defines validation rules for cart updating.
/// </summary>
public class UpdateCartRequestValidator : AbstractValidator<UpdateCartRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateCartRequestValidator with defined validation rules.
    /// </summary>
    public UpdateCartRequestValidator()
    {
        RuleFor(x => x.UserId)
         .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.Products)
            .NotEmpty().WithMessage("The cart must contain at least one product.");
    }
}
