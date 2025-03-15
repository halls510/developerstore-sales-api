using Ambev.DeveloperEvaluation.Application.Sales.CancelItem;
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

public class CancelItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelItemHandler> _logger;
    private readonly IBus _bus;
    private readonly CancelItemHandler _handler;

    public CancelItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<CancelItemHandler>>();
        _bus = Substitute.For<IBus>();
        _handler = new CancelItemHandler(_saleRepository, _mapper, _logger, _bus);
    }

    [Fact(DisplayName = "Given valid cancel request When processing Then returns cancelled item result")]
    public async Task Handle_ValidRequest_ReturnsCancelledItem()
    {
        var command = CancelItemHandlerTestData.GenerateValidCommand();
        var sale = CancelItemHandlerTestData.GenerateValidSale(command);
        var saleItem = sale.Items.FirstOrDefault(i => i.ProductId == command.ProductId);

        var expectedResult = new CancelItemResult
        {
            Id = saleItem.Id,
            SaleId = sale.Id,
            ProductId = saleItem.ProductId,
            ProductName = saleItem.ProductName,
            Quantity = saleItem.Quantity,
            UnitPrice = saleItem.UnitPrice,
            Discount = saleItem.Discount,
            Total = saleItem.Total,
            Status = saleItem.Status
        };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(sale, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelItemResult>(sale.Items.First()).Returns(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.ProductId.Should().Be(command.ProductId);
    }

    [Fact(DisplayName = "Given non-existing sale When cancelling item Then throws ResourceNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsException()
    {
        var command = CancelItemHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage("Sale does not exist.");
    }

    [Fact(DisplayName = "Given already cancelled item When cancelling again Then throws BusinessRuleException")]    
    public async Task Handle_AlreadyCancelledItem_ThrowsException()
    {
        // Arrange
        var command = CancelItemHandlerTestData.GenerateValidCommand();
        var sale = CancelItemHandlerTestData.GenerateSaleWithCancelledItem(command); // Venda com item já cancelado

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns(sale);

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessRuleException>().WithMessage("Item is already cancelled or returned.");
    }

}
