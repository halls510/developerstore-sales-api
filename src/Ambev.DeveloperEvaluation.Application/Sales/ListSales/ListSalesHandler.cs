using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales
{
    /// <summary>
    /// Handler para listar vendas
    /// </summary>
    public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ListSalesHandler> _logger;

        public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<ListSalesHandler> logger)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ListSalesResult> Handle(ListSalesCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando listagem de vendas - Página {Page}, Tamanho {Size}, Ordenação {OrderBy}",
                command.Page, command.Size, command.OrderBy ?? "default");

            var validator = new ListSalesCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Falha na validação do comando ListSalesCommand");
                throw new ValidationException(validationResult.Errors);
            }

            _logger.LogInformation("Buscando vendas do banco de dados...");
            var sales = await _saleRepository.GetSalesAsync(command.Page, command.Size, command.OrderBy, command.Filters, cancellationToken);
            var totalSales = await _saleRepository.CountSalesAsync(command.Filters, cancellationToken);

            _logger.LogInformation("Aplicando regras de negócio para filtragem de vendas canceladas...");
            sales = sales.Where(sale => OrderRules.CanSaleBeRetrieved(sale.Status, throwException: false)).ToList();

            _logger.LogInformation("Listagem de vendas concluída com {TotalSales} vendas encontradas", sales.Count);

            return new ListSalesResult
            {
                Sales = _mapper.Map<List<GetSaleResult>>(sales),
                TotalItems = totalSales,
                CurrentPage = command.Page,
                PageSize = command.Size
            };
        }
    }
}
