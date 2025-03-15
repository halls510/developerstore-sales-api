using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Repositories;

public class UserRepositoryTests
{
    private readonly IUserRepository _userRepository;
    private readonly CancellationToken _cancellationToken;

    public UserRepositoryTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _cancellationToken = new CancellationToken();
    }

    [Fact(DisplayName = "ExistsAsync should return true when user exists")]
    public async Task Given_ExistingUserId_When_ExistsAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _userRepository.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _userRepository.ExistsAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "CreateAsync should return created user")]
    public async Task Given_NewUser_When_CreateAsyncCalled_Then_ShouldReturnCreatedUser()
    {
        // Arrange
        var newUser = new User { Id = 1, Email = "newuser@example.com" };
        _userRepository.CreateAsync(newUser, Arg.Any<CancellationToken>()).Returns(newUser);

        // Act
        var result = await _userRepository.CreateAsync(newUser);

        // Assert
        result.Should().BeEquivalentTo(newUser);
    }

    [Fact(DisplayName = "GetByIdAsync should return user when id exists")]
    public async Task Given_ExistingUserId_When_GetByIdAsyncCalled_Then_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new User { Id = 1, Email = "test@example.com" };
        _userRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(expectedUser);

        // Act
        var result = await _userRepository.GetByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact(DisplayName = "GetByEmailAsync should return user when email exists")]
    public async Task Given_ExistingUserEmail_When_GetByEmailAsyncCalled_Then_ShouldReturnUser()
    {
        // Arrange
        var expectedUser = new User { Id = 1, Email = "test@example.com" };
        _userRepository.GetByEmailAsync("test@example.com", Arg.Any<CancellationToken>()).Returns(expectedUser);

        // Act
        var result = await _userRepository.GetByEmailAsync("test@example.com");

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact(DisplayName = "UpdateAsync should return updated user")]
    public async Task Given_ExistingUser_When_UpdateAsyncCalled_Then_ShouldReturnUpdatedUser()
    {
        // Arrange
        var updatedUser = new User { Id = 1, Email = "updated@example.com" };
        _userRepository.UpdateAsync(updatedUser, Arg.Any<CancellationToken>()).Returns(updatedUser);

        // Act
        var result = await _userRepository.UpdateAsync(updatedUser);

        // Assert
        result.Should().BeEquivalentTo(updatedUser);
    }

    [Fact(DisplayName = "DeleteAsync should return true when user is deleted")]
    public async Task Given_ExistingUserId_When_DeleteAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _userRepository.DeleteAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _userRepository.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "GetUsersAsync should return list of users")]
    public async Task Given_Pagination_When_GetUsersAsyncCalled_Then_ShouldReturnUserList()
    {
        // Arrange
        var users = new List<User> { new User { Id = 1, Email = "user1@example.com" } };
        _userRepository.GetUsersAsync(1, 10, null, null, Arg.Any<CancellationToken>()).Returns(users);

        // Act
        var result = await _userRepository.GetUsersAsync(1, 10, null, null, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(users);
    }

    [Fact(DisplayName = "CountUsersAsync should return total user count")]
    public async Task Given_Filters_When_CountUsersAsyncCalled_Then_ShouldReturnUserCount()
    {
        // Arrange
        _userRepository.CountUsersAsync(null, Arg.Any<CancellationToken>()).Returns(100);

        // Act
        var result = await _userRepository.CountUsersAsync(null, _cancellationToken);

        // Assert
        result.Should().Be(100);
    }
}