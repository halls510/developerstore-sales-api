using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale
{
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
            // 📌 1️⃣ Validar o comando com FluentValidation
            var validator = new CancelSaleCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Buscar a venda existente
            var existingSale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (existingSale == null)
                throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");

            // Verificar se a venda já está cancelada
            if (existingSale.IsCancelled())
                throw new BusinessRuleException("Sale is already cancelled.");

            // Marcar a venda e os items como cancelados
            existingSale.Cancel();

            // Salvar a venda atualizada
            var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

            // Log da operação
            _logger.LogInformation($"Sale {request.SaleId} has been cancelled.");

            // Publicar evento de Venda Criada
            var saleEvent = new SaleCancelledEvent(updatedSale);
            // LOG para verificar se o evento está sendo publicado
            _logger.LogInformation($"Publicando evento SaleCancelledEvent para venda ID {updatedSale.Id}");
            await _bus.Publish(saleEvent);

            // Mapear para o resultado esperado
            var result = _mapper.Map<CancelSaleResult>(updatedSale);
            return result;
        }
    }
}
