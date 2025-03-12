using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales
{
    /// <summary>
    /// Handler para listar vendas
    /// </summary>
    public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<ListSalesResult> Handle(ListSalesCommand command, CancellationToken cancellationToken)
        {
            var validator = new ListSalesCommandValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var sales = await _saleRepository.GetSalesAsync(command.Page, command.Size, command.OrderBy, command.Filters, cancellationToken);
            var totalSales = await _saleRepository.CountSalesAsync(command.Filters, cancellationToken);

            // Aplicando regras de negócio, por exemplo, filtrando vendas canceladas
            sales = sales.Where(sale => OrderRules.CanSaleBeRetrieved(sale.Status, throwException: false)).ToList();

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
