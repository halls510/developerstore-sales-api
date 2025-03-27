using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelItem;

public class CancelItemCommand : IRequest<CancelItemResult>
{
    public int SaleId { get; set; }
    public int ProductId { get; set; }

    public CancelItemCommand()
    {        
    }

    /// <summary>
    /// Initializes a new instance of CancelItemCommand
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="productId">The ID of the product to cancel</param>
    public CancelItemCommand(int saleId, int productId)
    {
        SaleId = saleId;
        ProductId = productId;
    }
}
