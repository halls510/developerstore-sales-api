using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ListUsersHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ListUsersHandler _handler;
    private readonly ILogger<ListUsersHandler> _logger;

    public ListUsersHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListUsersHandler>>();
        _handler = new ListUsersHandler(_userRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid list request When listing users Then returns paginated users")]
    public async Task Handle_ValidRequest_ReturnsUsersList()
    {
        var command = ListUsersHandlerTestData.GenerateValidCommand();
        var users = ListUsersHandlerTestData.GenerateUsersList();

        _userRepository.GetUsersAsync(command.Page, command.Size, command.OrderBy, command.Filters, Arg.Any<CancellationToken>())
            .Returns(users);
        _userRepository.CountUsersAsync(command.Filters, Arg.Any<CancellationToken>()).Returns(users.Count);
        _mapper.Map<List<GetUserResult>>(users).Returns(new List<GetUserResult>());
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Users.Should().NotBeNull();
        result.TotalItems.Should().Be(users.Count);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }
}
