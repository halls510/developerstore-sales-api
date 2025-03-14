using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Exceptions;

public class BusinessRuleExceptionTests
{
    [Fact(DisplayName = "BusinessRuleException should contain correct message")]
    public void Given_BusinessRuleException_When_Thrown_Then_ShouldContainCorrectMessage()
    {
        // Arrange
        var message = "A business rule has been violated.";

        // Act
        var exception = new BusinessRuleException(message);

        // Assert
        exception.Message.Should().Be(message);
    }

    [Fact(DisplayName = "BusinessRuleException should retain inner exception")]
    public void Given_BusinessRuleExceptionWithInnerException_When_Thrown_Then_ShouldContainInnerException()
    {
        // Arrange
        var message = "A business rule has been violated.";
        var innerException = new Exception("Inner exception details.");

        // Act
        var exception = new BusinessRuleException(message, innerException);

        // Assert
        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}