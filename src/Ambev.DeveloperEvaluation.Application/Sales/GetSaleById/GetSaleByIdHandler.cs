using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Application.Common;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

/// <summary>
/// Handler para processar a consulta GetSaleByIdQuery.
/// </summary>
public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleByIdHandler> _logger;

    public GetSaleByIdHandler(ISaleRepository saleRepository, IMapper mapper, ILogger<GetSaleByIdHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando busca da venda {SaleId}", request.Id);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
        {
            _logger.LogWarning("Venda {SaleId} não encontrada", request.Id);
            throw new ResourceNotFoundException("Sale not found", $"Sale with ID {request.Id} not found");
        }

        _logger.LogInformation("Venda {SaleId} recuperada com sucesso", request.Id);
        return _mapper.Map<SaleDto>(sale);
    }
}
