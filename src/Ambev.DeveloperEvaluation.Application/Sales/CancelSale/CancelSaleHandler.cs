using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;
    private readonly IBus _bus;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CancelSaleHandler> logger,
        IBus bus)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
        _bus = bus;
    }

    public async Task<CancelSaleResult> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        // Validar o comando com FluentValidation
        var validator = new CancelSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Buscar a venda existente
        var existingSale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (existingSale == null)
            throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");

        // Aplicar regra de negócio para verificar se a venda pode ser cancelada
        OrderRules.CanSaleBeCancelled(existingSale.Status, throwException: true);

        // Marcar a venda e os itens como cancelados
        existingSale.Cancel();

        // Salvar a venda atualizada no repositório
        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        // Log da operação
        _logger.LogInformation($"Venda {request.SaleId} foi cancelada.");

        // Publicar evento de Venda Cancelada
        var saleEvent = new SaleCancelledEvent(updatedSale);
        _logger.LogInformation($"Publicando evento SaleCancelledEvent para venda ID {updatedSale.Id}");
        await _bus.Publish(saleEvent);

        // Mapear para o resultado esperado e retornar
        var result = _mapper.Map<CancelSaleResult>(updatedSale);
        return result;
    }
}
