using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelItem;

public class CancelItemRequestValidator : AbstractValidator<CancelItemRequest>
{
    /// <summary>
    /// Initializes validation rules for CancelItemRequest
    /// </summary>
    public CancelItemRequestValidator()
    {
        RuleFor(x => x.SaleId)
            .NotEmpty()
            .WithMessage("Sale ID is required");

        RuleFor(x => x.ProductId)
           .NotEmpty()
           .WithMessage("Product ID is required");
    }
}
