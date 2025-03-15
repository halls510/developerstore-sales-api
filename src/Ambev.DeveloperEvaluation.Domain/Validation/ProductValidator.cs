using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator()
    {
        RuleFor(product => product.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Length(3, 200).WithMessage("Title must be between 3 and 200 characters.");

        RuleFor(product => product.Price.Amount)
            .GreaterThan(0).WithMessage("Price must be greater than zero and cannot be negative.");

        RuleFor(product => product.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(product => product.CategoryId)
          .NotEmpty().WithMessage("CategoryId is required.")
          .GreaterThan(0).WithMessage("CategoryId must be a valid integer greater than zero.");

        RuleFor(product => product.Image)
            .NotEmpty().WithMessage("Image URL is required.")
            .Matches(@"^(http|https):\/\/.*\.(jpg|jpeg|png|gif|bmp|webp)$")
            .WithMessage("Image must be a valid URL pointing to an image file (jpg, png, etc.).");

        RuleFor(product => product.Rating.Rate)
            .InclusiveBetween(0, 5).WithMessage("Rating must be between 0 and 5.");

        RuleFor(product => product.Rating.Count)
            .GreaterThanOrEqualTo(0).WithMessage("Rating count must be zero or greater.");       

        // Garantir que a imagem tenha um tamanho máximo de URL
        RuleFor(product => product.Image)
            .MaximumLength(500)
            .WithMessage("Image URL cannot exceed 500 characters.");

        // ** Validação da Categoria **
        RuleFor(product => product.Category)
            .NotNull().WithMessage("Category must be provided.");

        RuleFor(product => product.Category.Name)
            .NotEmpty().WithMessage("Category name must not be empty.")
            .Length(3, 100).WithMessage("Category name must be between 3 and 100 characters.");

        // Garantir que a data de criação seja válida
        RuleFor(product => product.CreatedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("CreatedAt must be a valid date in the past.");

        // Garantir que a data de atualização, se presente, seja posterior à data de criação
        RuleFor(product => product.UpdatedAt)
            .GreaterThanOrEqualTo(product => product.CreatedAt)
            .When(product => product.UpdatedAt.HasValue)
            .WithMessage("UpdatedAt must be later than CreatedAt.");
    }
}
