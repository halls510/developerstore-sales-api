using Ambev.DeveloperEvaluation.Application.Carts.Common;
using Ambev.DeveloperEvaluation.Application.Carts.GetCartById;
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

public class GetCartByIdHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetCartByIdHandler> _logger;
    private readonly GetCartByIdHandler _handler;

    public GetCartByIdHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetCartByIdHandler>>();
        _handler = new GetCartByIdHandler(_cartRepository, _mapper, _logger);
    }

    [Fact]
    public async Task Handle_ValidCartId_ReturnsCart()
    {
        var query = GetCartByIdHandlerTestData.GenerateValidQuery();
        var cart = GetCartByIdHandlerTestData.GenerateValidCart(query);
        var expectedResult = new CartDto { Id = cart.Id, CustomerId = cart.UserId };

        _cartRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult(cart));
        _mapper.Map<CartDto>(cart).Returns(expectedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(expectedResult.Id, result.Id);
        Assert.Equal(expectedResult.CustomerId, result.CustomerId);
    }

    [Fact]
    public async Task Handle_InvalidCartId_ThrowsResourceNotFoundException()
    {
        var query = GetCartByIdHandlerTestData.GenerateValidQuery();
        _cartRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Cart>(null));

        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var query = new GetCartByIdQuery(0); // ID inválido
        await Assert.ThrowsAsync<ResourceNotFoundException>(() => _handler.Handle(query, CancellationToken.None));
    }
}

