namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.Checkout;

/// <summary>
/// Representa a requisição para finalizar o checkout de um carrinho.
/// </summary>
public class CheckoutRequest
{
    /// <summary>
    /// O identificador do carrinho a ser finalizado.
    /// </summary>
    public int CartId { get; set; }
}

