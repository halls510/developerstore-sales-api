using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

/// <summary>
/// Contains unit tests for the SaleModifiedEvent class.
/// </summary>
public class SaleModifiedEventTests
{
    [Fact(DisplayName = "SaleModifiedEvent should be instantiated correctly with a valid sale")]
    public void Given_ValidSale_When_CreatingSaleModifiedEvent_Then_ShouldContainCorrectSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var saleModifiedEvent = new SaleModifiedEvent(sale);

        // Assert
        saleModifiedEvent.Sale.Should().BeEquivalentTo(sale);
    }

    [Fact(DisplayName = "SaleModifiedEvent should throw exception when instantiated with null sale")]
    public void Given_NullSale_When_CreatingSaleModifiedEvent_Then_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new SaleModifiedEvent(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Sale cannot be null.*");
    }
}