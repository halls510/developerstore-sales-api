using Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Uploads;

public class UploadRequestValidator : AbstractValidator<UploadRequest>
{
    /// <summary>
    /// Initializes a new instance of the UploadRequestValidator with defined validation rules.
    /// </summary>
    public UploadRequestValidator()
    {
        RuleFor(upload => upload.File)
            .NotEmpty();

    }
}
