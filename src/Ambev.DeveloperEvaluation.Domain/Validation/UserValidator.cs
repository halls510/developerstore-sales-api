using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Email).SetValidator(new EmailValidator());

        RuleFor(user => user.Username)
            .NotEmpty()
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.");

        RuleFor(user => user.Password).SetValidator(new PasswordValidator());

        RuleFor(user => user.Phone).SetValidator(new PhoneValidator());

        RuleFor(user => user.Status)
            .NotEqual(UserStatus.Unknown)
            .WithMessage("User status cannot be Unknown.");

        RuleFor(user => user.Role)
            .NotEqual(UserRole.None)
            .WithMessage("User role cannot be None.");



        RuleFor(user => user.Firstname)
           .NotEmpty().WithMessage("Firstname is required.")
           .MaximumLength(24).WithMessage("Firstname cannot exceed 24 characters.");

        RuleFor(user => user.Lastname)
            .NotEmpty().WithMessage("Lastname is required.")
            .MaximumLength(24).WithMessage("Lastname cannot exceed 24 characters.");

        RuleFor(user => user.Address.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(user => user.Address.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(100).WithMessage("Street cannot exceed 100 characters.");

        RuleFor(user => user.Address.Number)
         .GreaterThanOrEqualTo(0).WithMessage("Number must be zero or greater.");

        RuleFor(user => user.Address.Zipcode)
            .NotEmpty().WithMessage("Zipcode is required.")
            .MaximumLength(20).WithMessage("Zipcode cannot exceed 20 characters.");

        
        RuleFor(user => user.Address.Geolocation.Lat)
            .Must(lat => !double.IsNaN(lat)) 
            .WithMessage("Latitude cannot be NaN.")
            .InclusiveBetween(-90.0000, 90.0000) 
            .WithMessage("Latitude must be between -90.0000 and 90.0000.");

        
        RuleFor(user => user.Address.Geolocation.Long)
            .Must(lng => !double.IsNaN(lng)) 
            .WithMessage("Longitude cannot be NaN.")
            .InclusiveBetween(-180.0000, 180.0000) 
            .WithMessage("Longitude must be between -180.0000 and 180.0000.");

    }
}
