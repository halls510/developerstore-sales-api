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

    [Fact(DisplayName = "Given valid create request When creating cart Then returns created cart result")]
    public async Task Handle_ValidRequest_ReturnsCreatedCart()
    {
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var user = CreateCartHandlerTestData.GenerateValidUser(command.UserId);
        var products = CreateCartHandlerTestData.GenerateValidProducts(command.Items);
        var cart = CreateCartHandlerTestData.GenerateValidCart(command, user, products);
        var expectedResult = _mapper.Map<CreateCartResult>(cart);

        // Configurar o mock para que o mapeamento do comando para Cart funcione
        var mappedCart = new Cart
        {

            UserId = command.UserId,
            UserName = string.Format("{0} {1}", user.Firstname, user.Lastname),
            Items = cart.Items,
            TotalPrice = cart.TotalPrice,
            Status = cart.Status,
            Date = cart.Date
        };

        var mappedCartResult = new CreateCartResult
        {
            UserId = mappedCart.UserId,
            Products = mappedCart.Items.Select(item => new CartItemResult
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Total = item.Total
            }).ToList(),
            Status = mappedCart.Status,
            Date = mappedCart.Date
        };

        _mapper.Map<Cart>(command).Returns(mappedCart);
        _mapper.Map<CreateCartResult>(mappedCart).Returns(mappedCartResult);

        _userRepository.GetByIdAsync(command.UserId, Arg.Any<CancellationToken>()).Returns(user);
        _productRepository.GetByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>()).Returns(products);
        _cartRepository.CreateAsync(mappedCart, Arg.Any<CancellationToken>()).Returns(mappedCart);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.UserId.Should().Be(cart.UserId);
        result.Products.Should().BeEquivalentTo(mappedCartResult.Products);
        result.Status.Should().Be(cart.Status);
        result.Date.Should().Be(cart.Date);
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
