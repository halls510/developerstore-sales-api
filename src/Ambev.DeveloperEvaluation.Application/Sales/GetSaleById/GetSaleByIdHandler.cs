using MediatR;
using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Application.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

/// <summary>
/// Handler para processar a consulta GetSaleByIdQuery.
/// </summary>
public class GetSaleByIdHandler : IRequestHandler<GetSaleByIdQuery, SaleDto>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSaleByIdHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<SaleDto> Handle(GetSaleByIdQuery request, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale == null)
        {
            throw new ResourceNotFoundException("Sale not found", $"Sale with ID {request.Id} not found");
        }

        return _mapper.Map<SaleDto>(sale);
    }
}
