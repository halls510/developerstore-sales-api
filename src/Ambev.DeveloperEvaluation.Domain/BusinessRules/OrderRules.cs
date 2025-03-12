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
    private static readonly decimal Discount10Percent = 0.10m;
    private static readonly decimal Discount20Percent = 0.20m;

    /// <summary>
    /// Valida se a quantidade de itens está dentro do limite permitido.
    /// </summary>
    public static bool ValidateItemQuantity(int quantity)
    {
        return quantity > 0 && quantity <= MaxItemsPerProduct;
    }

    /// <summary>
    /// Calcula o valor absoluto do desconto aplicado a um item do carrinho.
    /// </summary>
    /// <param name="quantity">Quantidade do item</param>
    /// <param name="unitPrice">Preço unitário</param>
    /// <returns>Valor do desconto aplicado</returns>
    public static Money CalculateDiscount(int quantity, Money unitPrice)
    {
        if (quantity > MaxItemsPerProduct)
            throw new BusinessRuleException("Não é permitido mais de 20 itens do mesmo produto.");

        decimal discountRate = 0;
        if (quantity >= 10) discountRate = Discount20Percent;
        else if (quantity >= 4) discountRate = Discount10Percent;

        return unitPrice * discountRate * quantity;
    }

    /// <summary>
    /// Calcula o total de um item com desconto aplicado.
    /// </summary>
    public static Money CalculateTotalWithDiscount(int quantity, Money unitPrice)
    {
        var discount = CalculateDiscount(quantity, unitPrice);
        return (unitPrice * quantity) - discount;
    }

    /// <summary>
    /// Aplica o desconto ao preço unitário do item.
    /// </summary>
    public static Money ApplyDiscount(int quantity, Money unitPrice)
    {
        return CalculateTotalWithDiscount(quantity, unitPrice) / quantity;
    }

    /// <summary>
    /// Calcula o valor total do pedido considerando descontos aplicados.
    /// </summary>
    public static Money CalculateTotal(IEnumerable<(int Quantity, Money UnitPrice)> items)
    {
        return items.Sum(item => CalculateTotalWithDiscount(item.Quantity, item.UnitPrice));
    }

    /// <summary>
    /// Valida se um carrinho pode ser finalizado para checkout.
    /// </summary>
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
    /// Verifica se uma venda pode ser cancelada com base no status.
    /// </summary>
    /// <param name="status">Status da venda</param>
    /// <param name="throwException">Se verdadeiro, lança uma exceção quando a regra for violada.</param>
    /// <returns>Retorna verdadeiro se a venda puder ser cancelada</returns>
    public static bool CanSaleBeCancelled(SaleStatus status, bool throwException = true)
    {
        bool canCancel = status == SaleStatus.Pending || status == SaleStatus.Processing;

        if (!canCancel && throwException)
        {
            throw new BusinessRuleException("Only Pending or Processing sales can be cancelled.");
        }

        return canCancel;
    }

    /// <summary>
    /// Verifica se uma venda pode ser recuperada com base no status.
    /// </summary>
    public static bool CanSaleBeRetrieved(SaleStatus status, bool throwException = true)
    {
        bool canRetrieve = status is SaleStatus.Completed or SaleStatus.Processing or SaleStatus.Pending;

        if (!canRetrieve && throwException)
        {
            throw new BusinessRuleException("Esta venda não pode ser recuperada devido ao seu status atual.");
        }

        return canRetrieve;
    }


    /// <summary>
    /// Aplica todas as validações de negócio ao finalizar a venda.
    /// </summary>
    public static void ValidateSale(IEnumerable<(int Quantity, Money UnitPrice)> items)
    {
        if (!items.Any())
            throw new BusinessRuleException("Não é possível finalizar uma venda sem itens.");

        if (!items.All(i => ValidateItemQuantity(i.Quantity)))
            throw new BusinessRuleException("A venda contém itens com quantidade inválida.");
    }
}
