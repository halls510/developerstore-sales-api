using Ambev.DeveloperEvaluation.Application.Carts.Checkout;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class CheckoutHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutHandler> _logger;
    private readonly CheckoutHandler _handler;

    public CheckoutHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _saleRepository = Substitute.For<ISaleRepository>();
        //_bus = Substitute.For<IBus>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CheckoutHandler>>();
        _handler = new CheckoutHandler(_cartRepository,
            _saleRepository,
          //  _bus,
            _mapper,
            _logger);
    }

    [Fact(DisplayName = "Given valid checkout request When processing Then returns checkout result")]
    public async Task Handle_ValidRequest_ReturnsCheckoutResult()
    {
        var command = CheckoutHandlerTestData.GenerateValidCommand();
        var cart = CheckoutHandlerTestData.GenerateValidCart(command);
        var sale = CheckoutHandlerTestData.GenerateValidSale(cart);
        var expectedResult = new CheckoutResult
        {
            SaleId = sale.Id,
            TotalValue = sale.TotalValue
        };

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>()).Returns(cart);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(sale);
        _cartRepository.UpdateAsync(cart, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<CheckoutResult>(sale).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.SaleId.Should().Be(sale.Id);
        result.TotalValue.Should().Be(sale.TotalValue);
    }

    [Fact(DisplayName = "Given non-existing cart When processing checkout Then throws ResourceNotFoundException")]
    public async Task Handle_CartNotFound_ThrowsException()
    {
        var command = CheckoutHandlerTestData.GenerateValidCommand();

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>()).Returns((Cart)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage($"Carrinho {command.CartId} não encontrado.");
    }

    [Fact(DisplayName = "Given inactive cart When processing checkout Then throws Exception")]
    public async Task Handle_InactiveCart_ThrowsException()
    {
        var command = CheckoutHandlerTestData.GenerateValidCommand();
        var cart = CheckoutHandlerTestData.GenerateValidCart(command);
        cart.Status = CartStatus.Completed;

        _cartRepository.GetByIdAsync(command.CartId, Arg.Any<CancellationToken>()).Returns(cart);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<Exception>().WithMessage("O carrinho não pode ser finalizado pois não está ativo.");
    }
}

