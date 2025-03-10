using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem
{
    public class CancelItemHandler : IRequestHandler<CancelItemCommand, CancelItemResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CancelItemHandler> _logger;
        private readonly IBus _bus;

        public CancelItemHandler(
            ISaleRepository saleRepository,
            IMapper mapper,
            ILogger<CancelItemHandler> logger,
            IBus bus)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _logger = logger;
            _bus = bus;
        }

        public async Task<CancelItemResult> Handle(CancelItemCommand request, CancellationToken cancellationToken)
        {
            // Buscar a venda existente
            var existingSale = await _saleRepository.ExistsAsync(request.SaleId, cancellationToken);
            if (!existingSale)
                throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");

            // Criar uma cópia do item específico para evitar modificação na entidade Sale
            var saleItem = await _saleRepository.GetSaleItemByProductIdAsync(request.SaleId,request.ProductId, cancellationToken);
            if (saleItem == null)
                throw new ResourceNotFoundException("Item not found", "Item does not exist in the sale.");

            // Verificar se o item já está cancelado
            if (saleItem.Status == SaleItemStatus.Cancelled)
                throw new BusinessRuleException("Item is already cancelled.");

            // Cancelar o item sem afetar a entidade Sale
            saleItem.Cancel();

            // Atualiza o item para cancelado
            var updatedSale = await _saleRepository.UpdateItemAsync(saleItem, cancellationToken);

            // Log da operação
            _logger.LogInformation($"Item {saleItem.ProductId} da venda {saleItem.SaleId} foi cancelado.");

            // Publicar evento de Item Cancelado
            var itemEvent = new ItemCancelledEvent(saleItem);
            _logger.LogInformation($"Publicando evento ItemCancelledEvent para item {saleItem.ProductId} da venda ID {saleItem.SaleId}");
            await _bus.Publish(itemEvent);

            // Mapear para o resultado esperado
            var result = _mapper.Map<CancelItemResult>(saleItem);
            return result;
        }
    }
}
