using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class MoneyTests
{
    [Theory(DisplayName = "Money should allow only positive values")]
    [InlineData(0)]
    [InlineData(100)]
    public void Given_ValidAmount_When_CreatingMoney_Then_ShouldSucceed(decimal amount)
    {
        // Act
        var money = new Money(amount);

        // Assert
        money.Amount.Should().Be(amount);
    }

    [Fact(DisplayName = "Money should throw exception for negative values")]
    public void Given_NegativeAmount_When_CreatingMoney_Then_ShouldThrowException()
    {
        // Act
        Action act = () => new Money(-5);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>().WithMessage("*Money amount cannot be negative.*");
    }
}