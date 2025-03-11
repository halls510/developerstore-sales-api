using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

/// <summary>
/// Manipulador para processar o checkout do carrinho.
/// </summary>
public class CheckoutHandler : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CheckoutHandler(ICartRepository cartRepository, ISaleRepository saleRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        // Obtém o carrinho
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
            throw new Exception("Carrinho não encontrado.");

        // Extrai os itens do carrinho para aplicar regras
        var items = cart.Items.Select(i => (i.Quantity, i.UnitPrice)).ToList();

        // **Validações**
        OrderRules.ValidateCartForCheckout(items); // Garante que o carrinho é válido

        // **Aplica desconto e calcula total**
        var total = OrderRules.CalculateTotal(items);

        // **Atualiza o status do carrinho**
        cart.MarkAsCheckedOut(); // Método para atualizar o status no domínio

        // **Cria uma venda baseada no carrinho**
        var sale = _mapper.Map<Sale>(cart);
        sale.TotalValue = total; // Usa o total calculado

        // **Persiste a venda no banco de dados**
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // **Atualiza o carrinho como finalizado**
        await _cartRepository.UpdateAsync(cart, cancellationToken);

        return _mapper.Map<CheckoutResult>(createdSale);
    }
}
