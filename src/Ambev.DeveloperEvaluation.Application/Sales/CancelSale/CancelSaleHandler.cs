using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentValidation;
using Ambev.DeveloperEvaluation.Application.Common.Messaging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly RabbitMqPublisher _rabbitMqPublisher;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        RabbitMqPublisher rabbitMqPublisher,
        ILogger<CancelSaleHandler> logger
        )
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _rabbitMqPublisher = rabbitMqPublisher;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando cancelamento da venda {SaleId}", request.SaleId);

        // Validar o comando com FluentValidation
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando CancelSaleCommand para a venda {SaleId}", request.SaleId);
            throw new ValidationException(validationResult.Errors);
        }

        // Buscar a venda existente
        _logger.LogInformation("Buscando venda {SaleId} no banco de dados", request.SaleId);
        var existingSale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (existingSale == null)
        {
            _logger.LogWarning("Venda {SaleId} não encontrada", request.SaleId);
            throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");
        }

        // Aplicar regra de negócio para verificar se a venda pode ser cancelada
        _logger.LogInformation("Validando se a venda {SaleId} pode ser cancelada", request.SaleId);
        OrderRules.CanSaleBeCancelled(existingSale.Status, throwException: true);

        // Marcar a venda e os itens como cancelados
        _logger.LogInformation("Cancelando venda {SaleId} e seus itens", request.SaleId);
        existingSale.Cancel();

        // Salvar a venda atualizada no repositório
        _logger.LogInformation("Atualizando venda {SaleId} no banco de dados", request.SaleId);
        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        _logger.LogInformation("Venda {SaleId} foi cancelada com sucesso", request.SaleId);

        // Publicar evento de Venda Cancelada
        var saleEvent = new SaleCancelledEvent(updatedSale);
        _logger.LogInformation("📢 Publicando evento SaleCancelledEvent para venda ID {SaleId}", updatedSale.Id);
        await _rabbitMqPublisher.SendAsync(saleEvent);

        // Mapear para o resultado esperado e retornar
        _logger.LogInformation("Finalizando cancelamento da venda {SaleId}", request.SaleId);
        return _mapper.Map<CancelSaleResult>(updatedSale);
    }
}
