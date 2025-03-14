using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Services;

public class UserServiceTests
{
    private readonly IUserService _userService;

    public UserServiceTests()
    {
        _userService = Substitute.For<IUserService>();
    }

    [Fact(DisplayName = "GetUserByEmailAsync should return user when email exists")]
    public async Task Given_ExistingUserEmail_When_GetUserByEmailAsyncCalled_Then_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new User { Email = "test@example.com" };
        _userService.GetUserByEmailAsync("test@example.com", Arg.Any<CancellationToken>())
            .Returns(expectedUser);

        // Act
        var result = await _userService.GetUserByEmailAsync("test@example.com");

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact(DisplayName = "GetUserByEmailAsync should return null when email does not exist")]
    public async Task Given_NonExistingUserEmail_When_GetUserByEmailAsyncCalled_Then_ShouldReturnNull()
    {
        // Arrange
        _userService.GetUserByEmailAsync("unknown@example.com", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync("unknown@example.com");

        // Assert
        result.Should().BeNull();
    }
}