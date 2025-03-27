namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.Checkout;

// <summary>
/// Representa a resposta da finalização do checkout.
/// </summary>
public class CheckoutResponse
{
    public int SaleId { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalValue { get; set; }
    public string Status { get; set; } = "Completed";
}
