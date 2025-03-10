using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validates the CreateSaleRequest.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(s => s.CustomerId).GreaterThan(0);
        RuleFor(s => s.Items).NotEmpty();

        RuleForEach(s => s.Items).SetValidator(new CreateSaleItemRequestValidator());
    }
}

/// <summary>
/// Validates the items in the CreateSaleRequest.
/// </summary>
public class CreateSaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public CreateSaleItemRequestValidator()
    {
        RuleFor(i => i.ProductId).GreaterThan(0);
        RuleFor(i => i.Quantity).GreaterThan(0);
    }
}
