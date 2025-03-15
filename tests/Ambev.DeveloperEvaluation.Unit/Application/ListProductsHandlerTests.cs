using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ListProductsTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ListProductsHandler _handler;
    private readonly ILogger<ListProductsHandler> _logger;

    public ListProductsTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListProductsHandler>>();
        _handler = new ListProductsHandler(_productRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid list request When listing products Then returns paginated products")]
    public async Task Handle_ValidRequest_ReturnsProductsList()
    {
        var command = ListProductsHandlerTestData.GenerateValidCommand();
        var products = ListProductsHandlerTestData.GenerateProductsEntityList();
        var expectedProducts = ListProductsHandlerTestData.GenerateProductsList();

        _productRepository.GetProductsAsync(command.Page, command.Size, command.OrderBy, command.Filters, Arg.Any<CancellationToken>())
            .Returns(products);
        _productRepository.CountProductsAsync(command.Filters, Arg.Any<CancellationToken>()).Returns(products.Count);
        _mapper.Map<List<GetProductResult>>(products).Returns(expectedProducts);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Products.Should().NotBeNull();
        result.TotalItems.Should().Be(products.Count);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }
}