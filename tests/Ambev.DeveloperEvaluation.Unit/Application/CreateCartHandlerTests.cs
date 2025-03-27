using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CreateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCartHandler> _logger;
    private readonly CreateCartHandler _handler;
    private readonly IMediator _mediator;

    public CreateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CreateCartHandler>>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateCartHandler(_cartRepository, _userRepository, _productRepository, _mapper, _logger, _mediator);
    } 

    [Fact(DisplayName = "Given non-existing user When creating cart Then throws ResourceNotFoundException")]
    public async Task Handle_UserNotFound_ThrowsException()
    {
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns((User)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage("User does not exist.");
    }

    [Fact(DisplayName = "Given non-existing products When creating cart Then throws ResourceNotFoundException")]
    public async Task Handle_ProductNotFound_ThrowsException()
    {
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var user = CreateCartHandlerTestData.GenerateValidUser(command.UserId);
        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(user);

        var missingProductIds = command.Items.Select(i => i.ProductId).ToList();
        _productRepository.GetByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>()).Returns(new List<Product>());

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>()
            .WithMessage($"The following product(s) do not exist: {string.Join(", ", missingProductIds)}");
    }
}
