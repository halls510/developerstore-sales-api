using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

/// <summary>
/// Contains unit tests for the SaleCancelledEvent class.
/// </summary>
public class SaleCancelledEventTests
{
    [Fact(DisplayName = "SaleCancelledEvent should be instantiated correctly with a valid sale")]
    public void Given_ValidSale_When_CreatingSaleCancelledEvent_Then_ShouldContainCorrectSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var saleCancelledEvent = new SaleCancelledEvent(sale);

        // Assert
        saleCancelledEvent.Sale.Should().BeEquivalentTo(sale);
    }

    [Fact(DisplayName = "SaleCancelledEvent should throw exception when instantiated with null sale")]
    public void Given_NullSale_When_CreatingSaleCancelledEvent_Then_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new SaleCancelledEvent(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Sale cannot be null.*");
    }
}