using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

/// <summary>
/// Contains unit tests for the SaleCreatedEvent class.
/// </summary>
public class SaleCreatedEventTests
{
    [Fact(DisplayName = "SaleCreatedEvent should be instantiated correctly with a valid sale")]
    public void Given_ValidSale_When_CreatingSaleCreatedEvent_Then_ShouldContainCorrectSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var saleEvent = new SaleCreatedEvent(sale);

        // Assert
        saleEvent.Sale.Should().BeEquivalentTo(sale);
    }

    [Fact(DisplayName = "SaleCreatedEvent should throw exception when instantiated with null sale")]
    public void Given_NullSale_When_CreatingSaleCreatedEvent_Then_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new SaleCreatedEvent(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Sale cannot be null.*");
    }
}