using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Exceptions;

public class ResourceNotFoundExceptionTests
{
    [Fact(DisplayName = "ResourceNotFoundException should contain correct message and error")]
    public void Given_ResourceNotFoundException_When_Thrown_Then_ShouldContainCorrectDetails()
    {
        // Arrange
        var error = "UserNotFound";
        var detail = "The requested user was not found in the system.";

        // Act
        var exception = new ResourceNotFoundException(error, detail);

        // Assert
        exception.Message.Should().Be(detail);
        exception.Error.Should().Be(error);
    }
}