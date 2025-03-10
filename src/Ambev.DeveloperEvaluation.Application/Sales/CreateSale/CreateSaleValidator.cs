using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {        
        RuleFor(s => s.CustomerId).GreaterThan(0);
        RuleFor(s => s.Items).NotEmpty();
    }
}