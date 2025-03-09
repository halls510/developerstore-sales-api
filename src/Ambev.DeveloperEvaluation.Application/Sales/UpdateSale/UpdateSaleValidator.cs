using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleCommandValidator()
    {        
        RuleFor(s => s.CustomerId).GreaterThan(0);     
        RuleFor(s => s.Items).NotEmpty();
    }
}