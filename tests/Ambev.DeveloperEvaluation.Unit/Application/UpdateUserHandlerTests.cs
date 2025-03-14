using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly UpdateUserHandler _handler;
    private readonly ILogger<UpdateUserHandler> _logger;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _logger = Substitute.For<ILogger<UpdateUserHandler>>();
        _handler = new UpdateUserHandler(_userRepository, _mapper, _passwordHasher, _logger);
    }

    [Fact(DisplayName = "Given valid user update request When updating user Then returns updated user details")]
    public async Task Handle_ValidRequest_ReturnsUpdatedUser()
    {
        var command = UpdateUserHandlerTestData.GenerateValidCommand();
        var user = UpdateUserHandlerTestData.GenerateValidUser(command);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.UpdateAsync(user, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<UpdateUserResult>(user).Returns(new UpdateUserResult { Id = user.Id, Username = user.Username, Email = user.Email });

        var updateUserResult = await _handler.Handle(command, CancellationToken.None);

        updateUserResult.Should().NotBeNull();
        updateUserResult.Id.Should().Be(user.Id);
        updateUserResult.Username.Should().Be(user.Username);
        updateUserResult.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "Given invalid user ID When updating user Then throws ResourceNotFoundException")]
    public async Task Handle_InvalidUserId_ThrowsResourceNotFoundException()
    {
        var command = UpdateUserHandlerTestData.GenerateValidCommand();
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
