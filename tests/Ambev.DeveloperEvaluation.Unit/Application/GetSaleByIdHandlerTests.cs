using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleByIdHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleByIdHandler> _logger;
    private readonly GetSaleByIdHandler _handler;

    public GetSaleByIdHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetSaleByIdHandler>>();
        _handler = new GetSaleByIdHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale ID When retrieving sale Then returns sale details")]
    public async Task Handle_ValidRequest_ReturnsSaleDetails()
    {
        var query = GetSaleByIdHandlerTestData.GenerateValidQuery();
        var sale = GetSaleByIdHandlerTestData.GenerateValidSale(query);
        // Criar manualmente o objeto esperado para evitar null
        var expectedResult = new SaleDto
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            SaleDate = sale.SaleDate,
            Branch = sale.Branch,
            TotalValue = sale.TotalValue.Amount,
            Status = sale.Status,
            Items = sale.Items.Select(i => new SaleItemDto
            {
                SaleId = sale.Id,
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.Amount,
                Discount = i.Discount.Amount,
                Total = i.Total.Amount,
                Status = i.Status
            }).ToList()
        };

        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleDto>(sale).Returns(expectedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(query.Id);
    }

    [Fact(DisplayName = "Given non-existing sale ID When retrieving sale Then throws ResourceNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsException()
    {
        var query = GetSaleByIdHandlerTestData.GenerateValidQuery();

        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((Sale)null);

        var act = async () => await _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage($"Sale with ID {query.Id} not found");
    }
}
