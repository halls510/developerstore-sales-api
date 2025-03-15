using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Repositories;

public class CartRepositoryTests
{
    private readonly ICartRepository _cartRepository;
    private readonly CancellationToken _cancellationToken;

    public CartRepositoryTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _cancellationToken = new CancellationToken();
    }

    [Fact(DisplayName = "CreateAsync should return created cart")]
    public async Task Given_NewCart_When_CreateAsyncCalled_Then_ShouldReturnCreatedCart()
    {
        // Arrange
        var newCart = new Cart { Id = 1, UserId = 1001 };
        _cartRepository.CreateAsync(newCart, _cancellationToken).Returns(newCart);

        // Act
        var result = await _cartRepository.CreateAsync(newCart, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(newCart);
    }

    [Fact(DisplayName = "GetByIdAsync should return cart when id exists")]
    public async Task Given_ExistingCartId_When_GetByIdAsyncCalled_Then_ShouldReturnCart()
    {
        // Arrange
        var expectedCart = new Cart { Id = 1, UserId = 1001 };
        _cartRepository.GetByIdAsync(1, _cancellationToken).Returns(expectedCart);

        // Act
        var result = await _cartRepository.GetByIdAsync(1, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedCart);
    }

    [Fact(DisplayName = "DeleteAsync should return true when cart is deleted")]
    public async Task Given_ExistingCartId_When_DeleteAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _cartRepository.DeleteAsync(1, _cancellationToken).Returns(true);

        // Act
        var result = await _cartRepository.DeleteAsync(1, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "GetByUserIdAsync should return carts for user")]
    public async Task Given_ExistingUserId_When_GetByUserIdAsyncCalled_Then_ShouldReturnCarts()
    {
        // Arrange
        var carts = new List<Cart> { new Cart { Id = 1, UserId = 1001 } };
        _cartRepository.GetByUserIdAsync(1001, _cancellationToken).Returns(carts);

        // Act
        var result = await _cartRepository.GetByUserIdAsync(1001, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(carts);
    }

    [Fact(DisplayName = "GetCartsAsync should return list of carts")]
    public async Task Given_Pagination_When_GetCartsAsyncCalled_Then_ShouldReturnCartList()
    {
        // Arrange
        var carts = new List<Cart> { new Cart { Id = 1, UserId = 1001 } };
        _cartRepository.GetCartsAsync(1, 10, null, null, _cancellationToken).Returns(carts);

        // Act
        var result = await _cartRepository.GetCartsAsync(1, 10, null, null, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(carts);
    }
}