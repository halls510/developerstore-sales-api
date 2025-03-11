using Ambev.DeveloperEvaluation.Application.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

/// <summary>
/// Query para obter uma venda pelo ID.
/// </summary>
public record GetSaleByIdQuery : IRequest<SaleDto>
{
    /// <summary>
    /// O identificador único da venda.
    /// </summary>
    public int Id { get; init; }
}
