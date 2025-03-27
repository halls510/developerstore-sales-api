using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

/// <summary>
/// Contains unit tests for the UserRegisteredEvent class.
/// </summary>
public class UserRegisteredEventTests
{
    [Fact(DisplayName = "UserRegisteredEvent should be instantiated correctly with a valid user")]
    public void Given_ValidUser_When_CreatingUserRegisteredEvent_Then_ShouldContainCorrectUser()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();

        // Act
        var userRegisteredEvent = new UserRegisteredEvent(user);

        // Assert
        userRegisteredEvent.User.Should().BeEquivalentTo(user);
    }

    [Fact(DisplayName = "UserRegisteredEvent should allow null user")]
    public void Given_NullUser_When_CreatingUserRegisteredEvent_Then_ShouldAllowNull()
    {
        // Act
        var userRegisteredEvent = new UserRegisteredEvent(null);

        // Assert
        userRegisteredEvent.User.Should().BeNull();
    }
}