using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validates the UpdateSaleRequest.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {        
        RuleFor(s => s.CustomerId).GreaterThan(0);
        RuleFor(s => s.Items).NotEmpty();
        RuleForEach(s => s.Items).SetValidator(new UpdateSaleItemRequestValidator());
    }
}

/// <summary>
/// Validates the items in the UpdateSaleRequest.
/// </summary>
public class UpdateSaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public UpdateSaleItemRequestValidator()
    {
        RuleFor(i => i.ProductId).GreaterThan(0);
        RuleFor(i => i.Quantity).GreaterThan(0);
    }
}
