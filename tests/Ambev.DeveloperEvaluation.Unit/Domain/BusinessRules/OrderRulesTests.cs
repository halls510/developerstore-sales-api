using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.BusinessRules;

public class OrderRulesTests
{
    [Theory(DisplayName = "ValidateItemQuantity should return true for valid quantities")]
    [InlineData(1, true)]
    [InlineData(10, true)]
    [InlineData(20, true)]
    [InlineData(0, false)]
    [InlineData(21, false)]
    public void Given_Quantity_When_ValidateItemQuantityCalled_Then_ShouldReturnExpected(int quantity, bool expected)
    {
        var result = OrderRules.ValidateItemQuantity(quantity);
        result.Should().Be(expected);
    }

    [Fact(DisplayName = "CalculateDiscount should throw exception for quantity over max limit")]
    public void Given_ExcessiveQuantity_When_CalculateDiscountCalled_Then_ShouldThrowException()
    {
        var unitPrice = new Money(10);
        Action act = () => OrderRules.CalculateDiscount(21, unitPrice);
        act.Should().Throw<BusinessRuleException>().WithMessage("Não é permitido mais de 20 itens do mesmo produto.");
    }

    [Theory(DisplayName = "CalculateDiscount should apply correct discount rates")]
    [InlineData(3, 100, 0)]
    [InlineData(4, 100, 40)]
    [InlineData(10, 100, 200)]
    public void Given_QuantityAndPrice_When_CalculateDiscountCalled_Then_ShouldReturnCorrectDiscount(int quantity, decimal price, decimal expectedDiscount)
    {
        var unitPrice = new Money(price);
        var discount = OrderRules.CalculateDiscount(quantity, unitPrice);
        discount.Amount.Should().Be(expectedDiscount);
    }

    [Fact(DisplayName = "ValidateCartForCheckout should throw exception if cart is empty")]
    public void Given_EmptyCart_When_ValidateCartForCheckoutCalled_Then_ShouldThrowException()
    {
        Action act = () => OrderRules.ValidateCartForCheckout(new List<(int, Money)>());
        act.Should().Throw<BusinessRuleException>().WithMessage("O carrinho não pode estar vazio para finalizar o pedido.");
    }

    [Fact(DisplayName = "CanCartBeDeleted should return expected result based on status")]
    public void Given_CartStatus_When_CanCartBeDeletedCalled_Then_ShouldReturnExpected()
    {
        OrderRules.CanCartBeDeleted(CartStatus.Active).Should().BeTrue();
        OrderRules.CanCartBeDeleted(CartStatus.Cancelled).Should().BeTrue();
        OrderRules.CanCartBeDeleted(CartStatus.CheckedOut, false).Should().BeFalse();
    }

    [Fact(DisplayName = "CanSaleBeCancelled should throw exception for invalid status")]
    public void Given_InvalidSaleStatus_When_CanSaleBeCancelledCalled_Then_ShouldThrowException()
    {
        Action act = () => OrderRules.CanSaleBeCancelled(SaleStatus.Completed);
        act.Should().Throw<BusinessRuleException>().WithMessage("Only Pending or Processing sales can be cancelled.");
    }
}