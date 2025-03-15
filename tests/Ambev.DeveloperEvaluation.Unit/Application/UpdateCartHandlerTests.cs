using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateCartHandler> _logger;
    private readonly UpdateCartHandler _handler;

    public UpdateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<UpdateCartHandler>>();
        _handler = new UpdateCartHandler(_cartRepository, _userRepository, _productRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid update request When updating cart Then returns updated cart result")]
    public async Task Handle_ValidRequest_ReturnsUpdatedCart()
    {
        // Arrange
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        var existingCart = UpdateCartHandlerTestData.GenerateValidCart(command);
        var user = new User
        {
            Id = existingCart.UserId,
            Firstname = "John",
            Lastname = "Doe"
        };

        var products = command.Items.Select(i => new Product
        {
            Id = i.ProductId,
            Title = $"Product {i.ProductId}",
            Price = new Money(10) // Definindo um preço padrão para os produtos
        }).ToList();

        // Criar manualmente o objeto esperado para evitar null
        var expectedResult = new UpdateCartResult
        {
            Id = existingCart.Id,
            UserId = existingCart.UserId,
            Date = existingCart.Date,
            Status = existingCart.Status,
            Products = existingCart.Items.Select(i => new CartItemResult
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice.Amount,
                Discount = i.Discount.Amount,
                Total = i.Total.Amount
            }).ToList()
        };

        // Configuração dos mocks
        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingCart);
        _userRepository.GetByIdAsync(existingCart.UserId, Arg.Any<CancellationToken>()).Returns(user); // ✅ Corrigido para retornar um usuário válido
        _productRepository.GetByIdsAsync(Arg.Any<List<int>>(), Arg.Any<CancellationToken>()).Returns(products); // ✅ Retornando uma lista de produtos válidos
        _cartRepository.UpdateAsync(existingCart, Arg.Any<CancellationToken>()).Returns(existingCart);
        _mapper.Map<UpdateCartResult>(existingCart).Returns(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(existingCart.Id);
        result.UserId.Should().Be(existingCart.UserId);
        result.Status.Should().Be(existingCart.Status);
    }


    [Fact(DisplayName = "Given non-existing cart When updating cart Then throws ResourceNotFoundException")]
    public async Task Handle_CartNotFound_ThrowsException()
    {
        var command = UpdateCartHandlerTestData.GenerateValidCommand();

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Cart)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage("Cart does not exist.");
    }
}
