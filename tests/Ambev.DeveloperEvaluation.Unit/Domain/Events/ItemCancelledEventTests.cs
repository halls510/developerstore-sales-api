using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

/// <summary>
/// Contains unit tests for the ItemCancelledEvent class.
/// </summary>
public class ItemCancelledEventTests
{
    [Fact(DisplayName = "ItemCancelledEvent should be instantiated correctly with a valid sale item")]
    public void Given_ValidSaleItem_When_CreatingItemCancelledEvent_Then_ShouldContainCorrectSaleItem()
    {
        // Arrange
        var saleItem = SaleItemTestData.GenerateValidSaleItem();

        // Act
        var itemCancelledEvent = new ItemCancelledEvent(saleItem);

        // Assert
        itemCancelledEvent.SaleItem.Should().BeEquivalentTo(saleItem);
    }

    [Fact(DisplayName = "ItemCancelledEvent should throw exception when instantiated with null sale item")]
    public void Given_NullSaleItem_When_CreatingItemCancelledEvent_Then_ShouldThrowArgumentNullException()
    {
        // Act
        Action act = () => new ItemCancelledEvent(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("*Sale Item cannot be null.*");
    }
}