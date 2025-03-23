using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class UrlValidator : AbstractValidator<string>
{
    public UrlValidator()
    {
        RuleFor(url => url)
            .NotEmpty().WithMessage("URL cannot be empty.")
            .Matches(@"^https?:\/\/[a-zA-Z0-9\-.]+(:\d+)?(\/.*)?$")
            .WithMessage("Invalid URL format for the product image.");
    }
}
