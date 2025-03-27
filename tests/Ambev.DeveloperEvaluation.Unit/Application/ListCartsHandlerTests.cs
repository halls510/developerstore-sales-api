using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Application.Products.ListCarts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ListCartsHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ListCartsHandler _handler;
    private readonly ILogger<ListCartsHandler> _logger;

    public ListCartsHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListCartsHandler>>();
        _handler = new ListCartsHandler(_cartRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid list request When listing carts Then returns paginated carts")]
    public async Task Handle_ValidRequest_ReturnsCartsList()
    {
        // Given
        var command = ListCartsHandlerTestData.GenerateValidCommand();
        var carts = ListCartsHandlerTestData.GenerateCartsEntityList();
        var expectedCarts = ListCartsHandlerTestData.GenerateCartsList();

        carts.Should().NotBeNull();
        carts.Should().NotBeEmpty();

        _cartRepository.GetCartsAsync(command.Page, command.Size, command.OrderBy, command.Filters, Arg.Any<CancellationToken>())
            .Returns(carts);
        _cartRepository.CountCartsAsync(command.Filters, Arg.Any<CancellationToken>())
            .Returns(carts.Count);

        _mapper.Map<List<GetCartResult>>(Arg.Any<List<Cart>>()).Returns(expectedCarts);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Carts.Should().NotBeNull().And.NotBeEmpty();
        result.TotalItems.Should().Be(carts.Count);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }

}
