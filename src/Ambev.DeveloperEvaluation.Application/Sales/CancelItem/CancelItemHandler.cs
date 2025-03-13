using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;

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
            _logger.LogInformation("Iniciando cancelamento do item {ProductId} na venda {SaleId}", request.ProductId, request.SaleId);

            // Buscar a venda existente
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
            {
                _logger.LogWarning("Venda {SaleId} não encontrada", request.SaleId);
                throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");
            }

            // Verificar se a venda já foi finalizada ou enviada
            if (sale.Status == SaleStatus.Completed || sale.Status == SaleStatus.Shipped)
            {
                _logger.LogWarning("Tentativa de cancelamento de item {ProductId} em uma venda {SaleId} finalizada ou enviada", request.ProductId, request.SaleId);
                throw new BusinessRuleException("Cannot cancel an item from a completed or shipped sale.");
            }

            // Buscar o item da venda
            var saleItem = sale.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (saleItem == null)
            {
                _logger.LogWarning("Item {ProductId} não encontrado na venda {SaleId}", request.ProductId, request.SaleId);
                throw new ResourceNotFoundException("Item not found", "Item does not exist in the sale.");
            }

            // Verificar se o item já está cancelado ou devolvido
            if (saleItem.Status == SaleItemStatus.Cancelled || saleItem.Status == SaleItemStatus.Returned)
            {
                _logger.LogWarning("Item {ProductId} na venda {SaleId} já está cancelado ou devolvido", request.ProductId, request.SaleId);
                throw new BusinessRuleException("Item is already cancelled or returned.");
            }

            // Cancelar o item e atualizar status da venda
            _logger.LogInformation("Cancelando item {ProductId} na venda {SaleId}", request.ProductId, request.SaleId);
            saleItem.Cancel();

            // Recalcular total da venda após o cancelamento do item
            sale.RecalculateTotal();

            // Atualizar a venda no banco de dados
            _logger.LogInformation("Atualizando venda {SaleId} no banco de dados", sale.Id);
            var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);

            // Log da operação
            _logger.LogInformation("Item {ProductId} da venda {SaleId} foi cancelado com sucesso", saleItem.ProductId, sale.Id);

            // Publicar evento de Item Cancelado
            var itemEvent = new ItemCancelledEvent(saleItem);
            _logger.LogInformation("📢 Publicando evento ItemCancelledEvent para item {ProductId} da venda ID {SaleId}", saleItem.ProductId, sale.Id);
            await _bus.Publish(itemEvent);

            // Publicar evento de atualização da venda
            var saleUpdatedEvent = new SaleModifiedEvent(updatedSale);
            _logger.LogInformation("📢 Publicando evento SaleUpdatedEvent para venda ID {SaleId}", sale.Id);
            await _bus.Publish(saleUpdatedEvent);

            // Mapear para o resultado esperado e retornar
            _logger.LogInformation("Finalizando cancelamento do item {ProductId} na venda {SaleId}", request.ProductId, request.SaleId);
            return _mapper.Map<CancelItemResult>(saleItem);
        }
    }
}
