using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

/// <summary>
/// Command for retrieving a sale by their ID
/// </summary>
public record GetSaleCommand : IRequest<GetSaleResult>
{
    /// <summary>
    /// The unique identifier of the sale to retrieve
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Initializes a new instance of GetSaleCommand
    /// </summary>
    /// <param name="id">The ID of the sale to retrieve</param>
    public GetSaleCommand(int id)
    {
        Id = id;
    }
}
