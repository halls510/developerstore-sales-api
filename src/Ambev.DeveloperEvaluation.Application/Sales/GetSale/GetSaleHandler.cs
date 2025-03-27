using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Handler for processing GetSaleCommand requests
/// </summary>
public class GetSaleHandler : IRequestHandler<GetSaleCommand, GetSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetSaleHandler> _logger;

    /// <summary>
    /// Initializes a new instance of GetSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="logger">The ILogger instance</param>
    public GetSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<GetSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the GetSaleCommand request
    /// </summary>
    /// <param name="request">The GetSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    public async Task<GetSaleResult> Handle(GetSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando busca da venda {SaleId}", request.Id);

        var validator = new GetSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando GetSaleCommand para a venda {SaleId}", request.Id);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando venda {SaleId} no banco de dados", request.Id);
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
        {
            _logger.LogWarning("Venda {SaleId} não encontrada", request.Id);
            throw new ResourceNotFoundException("Sale not found", $"Sale with ID {request.Id} not found");
        }

        _logger.LogInformation("Venda {SaleId} recuperada com sucesso", request.Id);
        return _mapper.Map<GetSaleResult>(sale);
    }
}
