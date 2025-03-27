using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ListProductsByCategoryTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ListProductsByCategoryHandler _handler;
    private readonly ILogger<ListProductsByCategoryHandler> _logger;

    public ListProductsByCategoryTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListProductsByCategoryHandler>>();
        _handler = new ListProductsByCategoryHandler(_productRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid list request When listing products by category Then returns paginated products")]
    public async Task Handle_ValidRequest_ReturnsProductsList()
    {
        var command = ListProductsByCategoryHandlerTestData.GenerateValidCommand();
        var products = ListProductsByCategoryHandlerTestData.GenerateProductsEntityList();
        var expectedProducts = ListProductsByCategoryHandlerTestData.GenerateProductsList();

        _productRepository.GetProductsByCategoryAsync(command.CategoryName, command.Page, command.Size, command.OrderBy, Arg.Any<CancellationToken>())
            .Returns(products);
        _productRepository.CountProductsByCategoryAsync(command.CategoryName, Arg.Any<CancellationToken>()).Returns(products.Count);
        _mapper.Map<List<GetProductResult>>(products).Returns(expectedProducts);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Products.Should().NotBeNull();
        result.TotalItems.Should().Be(products.Count);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }
}