using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemCommandValidator : AbstractValidator<CancelItemCommand>
{
    public CancelItemCommandValidator()
    {
        RuleFor(x => x.SaleId)
           .NotEmpty()
           .WithMessage("Sale ID is required");

        RuleFor(x => x.ProductId)
           .NotEmpty()
           .WithMessage("Product ID is required");
    }
}