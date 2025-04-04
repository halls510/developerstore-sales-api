﻿using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;


/// <summary>
/// Validator for the Cart entity.
/// </summary>
public class CartValidator : AbstractValidator<Cart>
{
    public CartValidator()
    {
        RuleFor(cart => cart.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than zero.");

        RuleFor(cart => cart.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .MaximumLength(200).WithMessage("UserName cannot exceed 200 characters.");  

        RuleFor(cart => cart.Status)
            .IsInEnum().WithMessage("Invalid cart status.");

        RuleFor(cart => cart.Items)
            .NotEmpty().WithMessage("Cart must contain at least one item.");

        RuleForEach(cart => cart.Items).SetValidator(new CartItemValidator());
      
    }
}
