using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the Sale entity.
/// </summary>
public class SaleValidator : AbstractValidator<Sale>
{
    public SaleValidator()
    {
        RuleFor(sale => sale.SaleNumber)
            .NotEmpty().WithMessage("SaleNumber is required.")
            .MaximumLength(100).WithMessage("SaleNumber cannot exceed 100 characters.");

        RuleFor(sale => sale.SaleDate)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("SaleDate cannot be in the future.");

        RuleFor(sale => sale.CustomerId)
            .GreaterThan(0).WithMessage("CustomerId must be greater than zero.");

        RuleFor(sale => sale.CustomerName)
            .NotEmpty().WithMessage("CustomerName is required.")
            .MaximumLength(100).WithMessage("CustomerName cannot exceed 100 characters.");

        RuleFor(sale => sale.Branch)
            .NotEmpty().WithMessage("Branch is required.")
            .MaximumLength(50).WithMessage("Branch cannot exceed 50 characters.");

        RuleFor(sale => sale.Status)
            .IsInEnum().WithMessage("Invalid sale status.");

        RuleFor(sale => sale.TotalValue.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("TotalValue must be a positive value or zero.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("Sale must contain at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new SaleItemValidator());
    }
}