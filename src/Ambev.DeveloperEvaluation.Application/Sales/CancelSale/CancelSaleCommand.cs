using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    public int SaleId { get; set; }

    /// <summary>
    /// Initializes a new instance of CancelSaleCommand
    /// </summary>
    /// <param name="id">The ID of the sale to cancel</param>
    public CancelSaleCommand(int id)
    {
        SaleId = id;
    }
}
