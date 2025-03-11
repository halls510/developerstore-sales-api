using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.BusinessRules;

/// <summary>
/// Regras de negócio para Carrinho, Checkout e Venda.
/// </summary>
public static class OrderRules
{
    private const int MaxItemsPerProduct = 20;
    private static readonly decimal Discount10Percent = 0.90m;
    private static readonly decimal Discount20Percent = 0.80m;

    /// <summary>
    /// Valida se a quantidade de itens está dentro do limite permitido.
    /// </summary>
    /// <param name="quantity">Quantidade do item</param>
    /// <returns>Verdadeiro se a quantidade for válida</returns>
    public static bool ValidateItemQuantity(int quantity)
    {
        return quantity > 0 && quantity <= MaxItemsPerProduct;
    }

    /// <summary>
    /// Aplica desconto baseado na quantidade de itens comprados.
    /// </summary>
    /// <param name="quantity">Quantidade do item</param>
    /// <param name="unitPrice">Preço unitário</param>
    /// <returns>Preço com desconto aplicado</returns>
    public static Money ApplyDiscount(int quantity, Money unitPrice)
    {
        if (quantity > MaxItemsPerProduct)
            throw new BusinessRuleException("Não é permitido mais de 20 itens do mesmo produto.");

        if (quantity >= 10) return unitPrice * Discount20Percent; // 20% de desconto
        if (quantity >= 4) return unitPrice * Discount10Percent; // 10% de desconto

        return unitPrice; // Sem desconto
    }

    /// <summary>
    /// Calcula o valor total do pedido considerando descontos aplicados.
    /// </summary>
    /// <param name="items">Lista de itens (Quantidade, Preço Unitário)</param>
    /// <returns>Valor total do pedido</returns>
    public static Money CalculateTotal(IEnumerable<(int Quantity, Money UnitPrice)> items)
    {
        return items.Sum(item => item.Quantity * ApplyDiscount(item.Quantity, item.UnitPrice));
    }

    /// <summary>
    /// Valida se um carrinho pode ser finalizado para checkout.
    /// </summary>
    /// <param name="items">Lista de itens no carrinho</param>
    public static void ValidateCartForCheckout(IEnumerable<(int Quantity, Money UnitPrice)> items)
    {
        if (!items.Any())
            throw new BusinessRuleException("O carrinho não pode estar vazio para finalizar o pedido.");

        if (!items.All(i => ValidateItemQuantity(i.Quantity)))
            throw new BusinessRuleException("Quantidade inválida em um ou mais itens do carrinho.");
    }

    /// <summary>
    /// Verifica se um carrinho pode ser deletado.
    /// </summary>
    /// <param name="status">Status do carrinho</param>
    /// <param name="throwException">Se verdadeiro, lança uma exceção quando a regra for violada.</param>
    /// <returns>Retorna verdadeiro se o carrinho puder ser deletado</returns>
    public static bool CanCartBeDeleted(CartStatus status, bool throwException = true)
    {
        bool canDelete = status == CartStatus.Active || status == CartStatus.Cancelled;

        if (!canDelete && throwException)
        {
            throw new BusinessRuleException("Only Active or Cancelled carts can be deleted.");
        }

        return canDelete;
    }

    /// <summary>
    /// Verifica se um carrinho pode ser atualizado.
    /// </summary>
    /// <param name="status">Status do carrinho</param>
    /// <param name="throwException">Se verdadeiro, lança uma exceção quando a regra for violada.</param>
    /// <returns>Retorna verdadeiro se o carrinho puder ser atualizado</returns>
    public static bool CanCartBeUpdated(CartStatus status, bool throwException = true)
    {
        bool canUpdate = status != CartStatus.CheckedOut && status != CartStatus.Completed;

        if (!canUpdate && throwException)
        {
            throw new BusinessRuleException("Cart cannot be updated after checkout or completion.");
        }

        return canUpdate;
    }

    /// <summary>
    /// Verifica se um carrinho pode ser recuperado com base no status.
    /// </summary>
    /// <param name="status">Status do carrinho</param>
    /// <param name="throwException">Se verdadeiro, lança uma exceção quando a regra for violada.</param>
    /// <returns>Retorna verdadeiro se o carrinho puder ser recuperado</returns>
    public static bool CanCartBeRetrieved(CartStatus status, bool throwException = true)
    {
        bool canRetrieve = status is CartStatus.Active or CartStatus.CheckedOut or CartStatus.Completed;

        if (!canRetrieve && throwException)
        {
            throw new BusinessRuleException("This cart cannot be retrieved due to its current status.");
        }

        return canRetrieve;
    }



    /// <summary>
    /// Aplica todas as validações de negócio ao finalizar a venda.
    /// </summary>
    /// <param name="items">Lista de itens vendidos</param>
    public static void ValidateSale(IEnumerable<(int Quantity, Money UnitPrice)> items)
    {
        if (!items.Any())
            throw new BusinessRuleException("Não é possível finalizar uma venda sem itens.");

        if (!items.All(i => ValidateItemQuantity(i.Quantity)))
            throw new BusinessRuleException("A venda contém itens com quantidade inválida.");
    }
}
