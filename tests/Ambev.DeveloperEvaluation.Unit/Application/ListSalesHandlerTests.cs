using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;
    private readonly ILogger<ListSalesHandler> _logger;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListSalesHandler>>();
        _handler = new ListSalesHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid list request When listing sales Then returns paginated sales")]
    public async Task Handle_ValidRequest_ReturnsSalesList()
    {
        // Given
        var command = ListSalesHandlerTestData.GenerateValidCommand();
        var sales = ListSalesHandlerTestData.GenerateSalesEntityList();
        var expectedSales = ListSalesHandlerTestData.GenerateSalesList();

        sales.Should().NotBeNull();
        sales.Should().NotBeEmpty();  // Verifica se há dados antes do mapeamento

        _saleRepository.GetSalesAsync(command.Page, command.Size, command.OrderBy, command.Filters, Arg.Any<CancellationToken>())
            .Returns(sales);
        _saleRepository.CountSalesAsync(command.Filters, Arg.Any<CancellationToken>())
            .Returns(sales.Count);

        _mapper.Map<List<GetSaleResult>>(Arg.Any<List<Sale>>()).Returns(expectedSales); // Melhor compatibilidade com NSubstitute

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Sales.Should().NotBeNull().And.NotBeEmpty();
        result.TotalItems.Should().Be(sales.Count);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }
}
