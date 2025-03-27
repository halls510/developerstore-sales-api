using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleHandler> _logger;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<GetSaleHandler>>();
        _handler = new GetSaleHandler(_saleRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given valid sale ID When retrieving sale Then returns sale details")]
    public async Task Handle_ValidRequest_ReturnsSaleDetails()
    {
        // Arrange
        var command = GetSaleHandlerTestData.GenerateValidCommand();
        var sale = GetSaleHandlerTestData.GenerateValidSale(command);
    
        var expectedResult = new GetSaleResult(
            sale.Id,                        // SaleId
            sale.SaleNumber,                 // Número da venda
            sale.SaleDate,                   // Data da venda
            sale.CustomerId,                 // ID do cliente
            sale.CustomerName,               // Nome do cliente
            sale.TotalValue,                 // Valor total da venda (Money)
            sale.Branch,                      // Filial onde a venda ocorreu
            sale.Status.ToString(),           // Status da venda como string
            sale.Items.Select(i => new SaleItemResult(
                i.Id,                         // ID do item da venda
                i.SaleId,
                i.ProductId,                   // ID do produto
                i.ProductName,                 // Nome do produto
                i.Quantity,                    // Quantidade
                new Money(i.UnitPrice.Amount),  // Preço unitário (Money)
                new Money(i.Discount.Amount),   // Desconto aplicado (Money)
                new Money(i.Total.Amount),      // Total após desconto (Money)
                i.Status.ToString()             // Status do item como string
            )).ToList()
        );

        // Configuração dos mocks
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(expectedResult); // ✅ Agora retorna um objeto válido

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.SaleId.Should().Be(command.Id);
    }


    [Fact(DisplayName = "Given non-existing sale ID When retrieving sale Then throws ResourceNotFoundException")]
    public async Task Handle_SaleNotFound_ThrowsException()
    {
        var command = GetSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale)null);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ResourceNotFoundException>().WithMessage($"Sale with ID {command.Id} not found");
    }

    [Fact(DisplayName = "Given invalid command When retrieving sale Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        var command = new GetSaleCommand(0); // ID inválido

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
