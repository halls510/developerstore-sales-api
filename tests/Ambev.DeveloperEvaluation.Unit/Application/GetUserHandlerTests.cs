using Ambev.DeveloperEvaluation.Application.Users.GetUser;
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

public class GetUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly GetUserHandler _handler;
    private readonly ILogger<GetUserHandler> _logger;

    public GetUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetUserHandler>>();
        _handler = new GetUserHandler(_userRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid user ID When retrieving user Then returns user details")]
    public async Task Handle_ValidUserId_ReturnsUserDetails()
    {
        var command = GetUserHandlerTestData.GenerateValidCommand();
        var user = GetUserHandlerTestData.GenerateValidUser(command);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
        _mapper.Map<GetUserResult>(user).Returns(new GetUserResult { Id = user.Id, Username = user.Username, Email = user.Email });

        var getUserResult = await _handler.Handle(command, CancellationToken.None);

        getUserResult.Should().NotBeNull();
        getUserResult.Id.Should().Be(user.Id);
        getUserResult.Username.Should().Be(user.Username);
        getUserResult.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "Given invalid user ID When retrieving user Then throws ResourceNotFoundException")]
    public async Task Handle_InvalidUserId_ThrowsResourceNotFoundException()
    {
        var command = GetUserHandlerTestData.GenerateValidCommand();
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
