using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Exceptions;

public class AuthenticationErrorExceptionTests
{
    [Fact(DisplayName = "AuthenticationErrorException should contain correct message and error")]
    public void Given_AuthenticationErrorException_When_Thrown_Then_ShouldContainCorrectDetails()
    {
        // Arrange
        var error = "InvalidCredentials";
        var detail = "The provided credentials are incorrect.";

        // Act
        var exception = new AuthenticationErrorException(error, detail);

        // Assert
        exception.Message.Should().Be(detail);
        exception.Error.Should().Be(error);
    }
}