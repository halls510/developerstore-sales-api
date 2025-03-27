using MediatR;


namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

/// <summary>
/// Comando para processar o checkout do carrinho.
/// </summary>
public class CheckoutCommand : IRequest<CheckoutResult>
{
    /// <summary>
    /// ID do carrinho a ser finalizado.
    /// </summary>
    public int CartId { get; set; }
}

