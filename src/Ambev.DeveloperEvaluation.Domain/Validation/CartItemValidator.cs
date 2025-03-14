using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the CartItem entity.
/// </summary>
public class CartItemValidator : AbstractValidator<CartItem>
{
    public CartItemValidator()
    {
        RuleFor(item => item.CartId)
            .GreaterThan(0).WithMessage("CartId must be greater than zero.");

        RuleFor(item => item.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be greater than zero.");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("ProductName is required.")
            .MaximumLength(200).WithMessage("ProductName cannot exceed 200 characters.");

        RuleFor(item => item.UnitPrice.Amount)
            .GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");

        RuleFor(item => item.Discount.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative.");

        RuleFor(item => item.Total.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Total must be a positive value or zero.");
    }
}
