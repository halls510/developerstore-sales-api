using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
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

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly IBus _bus;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelSaleHandler>>();
        _bus = Substitute.For<IBus>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _logger, _bus);
    }

    [Fact(DisplayName = "Given valid cancel request When processing Then returns cancelled sale result")]
    public async Task Handle_ValidRequest_ReturnsCancelledSale()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = CancelSaleHandlerTestData.GenerateValidSale(command);
        var expectedResult = new CancelSaleResult
        {
            SaleId = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            CustomerId = sale.CustomerId,
            CustomerName = sale.CustomerName,
            TotalValue = sale.TotalValue,
            Branch = sale.Branch,
            Status = sale.Status.ToString(),
            Items = sale.Items.Select(i => new SaleItemResult(
                i.Id,                         // ID do item da venda
                i.SaleId,                       // ID da venda
                i.ProductId,                   // ID do produto
                i.ProductName,                 // Nome do produto
                i.Quantity,                    // Quantidade
                i.UnitPrice,                   // Preço unitário (Money)
                i.Discount,                     // Desconto aplicado (Money)
                i.Total,                        // Total após desconto (Money)
                i.Status.ToString()             // Status do item
            )).ToList()
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelSaleResult>(sale).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.SaleId.Should().Be(command.SaleId);
    }

    [Fact(DisplayName = "Given non-existing sale When cancelling Then throws ResourceNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsException()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage("Sale does not exist.");
    }

    [Fact(DisplayName = "Given already completed sale When cancelling Then throws BusinessRuleException")]
    public async Task Handle_CompletedSale_ThrowsException()
    {
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = CancelSaleHandlerTestData.GenerateValidCompletedSale(command);       
        
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns(sale);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("Only Pending or Processing sales can be cancelled.");
    }
}
