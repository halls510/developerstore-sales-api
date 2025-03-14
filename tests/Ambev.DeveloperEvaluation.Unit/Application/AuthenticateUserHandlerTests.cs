using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthenticateUserHandler _handler;
    private readonly ILogger<AuthenticateUserHandler> _logger;

    public AuthenticateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _logger = Substitute.For<ILogger<AuthenticateUserHandler>>();
        _handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);
    }

    [Fact(DisplayName = "Given valid credentials When authenticating user Then returns token")]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        var user = AuthenticateUserHandlerTestData.GenerateValidUser(command);
        var token = "valid_jwt_token";

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);
        _jwtTokenGenerator.GenerateToken(user).Returns(token);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Token.Should().Be(token);
    }

    [Fact(DisplayName = "Given invalid credentials When authenticating user Then throws UnauthorizedAccessException")]
    public async Task Handle_InvalidCredentials_ThrowsUnauthorizedAccessException()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "Given inactive user When authenticating Then throws UnauthorizedAccessException")]
    public async Task Handle_InactiveUser_ThrowsUnauthorizedAccessException()
    {
        // Given
        var command = AuthenticateUserHandlerTestData.GenerateValidCommand();
        var user = AuthenticateUserHandlerTestData.GenerateInactiveUser(command);

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
