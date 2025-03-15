using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCartHandler> _logger;
    private readonly GetCartHandler _handler;

    public GetCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetCartHandler>>();
        _handler = new GetCartHandler(_cartRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_ValidCartId_ReturnsCart()
    {
        var command = GetCartHandlerTestData.GenerateValidCommand();
        var cart = GetCartHandlerTestData.GenerateValidCart(command);
        var expectedResult = new GetCartResult { Id = cart.Id, UserId = cart.UserId, Status = cart.Status };

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(cart));
        _mapper.Map<GetCartResult>(cart).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.UserId, result.UserId);
        Assert.Equal(expectedResult.Status, result.Status);
    }

    [Fact]
    public async Task Handle_InvalidCartId_ThrowsResourceNotFoundException()
    {
        var command = GetCartHandlerTestData.GenerateValidCommand();
        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Cart>(null));

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = new GetCartCommand(0); // ID inválido
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
