using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the SaleItem entity.
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {       

        RuleFor(item => item.ProductId)
            .GreaterThan(0).WithMessage("ProductId must be greater than zero.");

        RuleFor(item => item.ProductName)
            .NotEmpty().WithMessage("ProductName is required.")
            .MaximumLength(100).WithMessage("ProductName cannot exceed 100 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.");

        RuleFor(item => item.UnitPrice.Amount)
            .GreaterThan(0).WithMessage("UnitPrice must be greater than zero.");

        RuleFor(item => item.Discount.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative.");

        RuleFor(item => item.Total.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Total must be a positive value or zero.");

        RuleFor(item => item.Status)
            .IsInEnum().WithMessage("Invalid sale item status.");
    }
}
