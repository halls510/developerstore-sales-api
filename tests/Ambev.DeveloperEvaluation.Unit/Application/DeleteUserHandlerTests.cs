using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
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

public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly DeleteUserHandler _handler;
    private readonly ILogger<DeleteUserHandler> _logger;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<DeleteUserHandler>>();
        _handler = new DeleteUserHandler(_userRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid user ID When deleting user Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        var command = DeleteUserHandlerTestData.GenerateValidCommand();
        var user = DeleteUserHandlerTestData.GenerateValidUser(command);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(user);
        _userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);
        _mapper.Map<DeleteUserResult>(user).Returns(new DeleteUserResult { Id = user.Id });

        var deleteUserResult = await _handler.Handle(command, CancellationToken.None);

        deleteUserResult.Should().NotBeNull();
        deleteUserResult.Id.Should().Be(user.Id);
        await _userRepository.Received(1).DeleteAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid user ID When deleting user Then throws ResourceNotFoundException")]
    public async Task Handle_InvalidUserId_ThrowsResourceNotFoundException()
    {
        var command = DeleteUserHandlerTestData.GenerateValidCommand();
        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((User)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>();
    }
}
