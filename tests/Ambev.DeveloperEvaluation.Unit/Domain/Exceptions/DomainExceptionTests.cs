using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact(DisplayName = "DomainException should contain correct message")]
    public void Given_DomainException_When_Thrown_Then_ShouldContainCorrectMessage()
    {
        // Arrange
        var message = "A domain-related error occurred.";

        // Act
        var exception = new DomainException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact(DisplayName = "DomainException should retain inner exception")]
    public void Given_DomainExceptionWithInnerException_When_Thrown_Then_ShouldContainInnerException()
    {
        // Arrange
        var message = "A domain-related error occurred.";
        var innerException = new Exception("Inner exception details.");

        // Act
        var exception = new DomainException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}