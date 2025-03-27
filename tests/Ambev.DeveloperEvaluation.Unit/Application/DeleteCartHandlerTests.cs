using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class DeleteCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DeleteCartHandler> _logger;
    private readonly DeleteCartHandler _handler;

    public DeleteCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<DeleteCartHandler>>();
        _handler = new DeleteCartHandler(_cartRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_Should_Delete_Cart_When_Valid()
    {
        var command = DeleteCartHandlerTestData.GenerateValidCommand();
        var cart = DeleteCartHandlerTestData.GenerateValidCart(command);

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _cartRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(true);
        _mapper.Map<DeleteCartResult>(cart).Returns(new DeleteCartResult());

        var result = await _handler.Handle(command, CancellationToken.None);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Handle_Should_Throw_ResourceNotFoundException_When_Cart_Not_Found()
    {
        var command = DeleteCartHandlerTestData.GenerateValidCommand();
        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Cart>(null));

        var exception = await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Equal($"Cart with ID {command.Id} not found", exception.Message);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Cart_Cannot_Be_Deleted()
    {
        var command = DeleteCartHandlerTestData.GenerateValidCommand();
        var cart = DeleteCartHandlerTestData.GenerateValidCart(command);

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(cart);
        _cartRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>()).Returns(false);

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
