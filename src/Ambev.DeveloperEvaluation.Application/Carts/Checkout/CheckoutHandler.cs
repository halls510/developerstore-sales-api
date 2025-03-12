using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using MediatR;
using Rebus.Bus;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

/// <summary>
/// Manipulador para processar o checkout do carrinho.
/// </summary>
public class CheckoutHandler : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IBus _bus;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutHandler> _logger;

    public CheckoutHandler(ICartRepository cartRepository, ISaleRepository saleRepository, IBus bus, IMapper mapper, ILogger<CheckoutHandler> logger)
    {
        _cartRepository = cartRepository;
        _saleRepository = saleRepository;
        _bus = bus;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        // 🔹 1️⃣ Obter o carrinho
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
            throw new Exception("Carrinho não encontrado.");

        // 🔹 2️⃣ Verificar se o carrinho pode ser finalizado
        if (cart.Status != CartStatus.Active)
            throw new Exception("O carrinho não pode ser finalizado pois não está ativo.");

        var items = cart.Items.Select(i => (i.Quantity, i.UnitPrice)).ToList();

        // 🔹 3️⃣ Aplicar regras de negócio
        OrderRules.ValidateCartForCheckout(items); // Validação

        // 🔹 4️⃣ Criar a venda baseada no carrinho
        var sale = new Sale(cart.UserId, cart.UserName);

        sale.AddItems(cart.Items.Select(cartItem =>
            new SaleItem(
                cartItem.ProductId,
                cartItem.ProductName,
                cartItem.Quantity,
                cartItem.UnitPrice,
                cartItem.Discount,
                cartItem.Total
            )).ToList());

        // 🔹 5️⃣ Definir o valor total com desconto aplicado
        sale.TotalValue = OrderRules.CalculateTotal(items);

        // 🔹 6️⃣ Persistir a venda no banco de dados
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // 🔹 7️⃣ Atualizar o status do carrinho para "CheckedOut"
        cart.MarkAsCheckedOut();
        await _cartRepository.UpdateAsync(cart, cancellationToken);

        // 🔹 8️⃣ Publicar o evento de venda criada
        var saleEvent = new SaleCreatedEvent(createdSale);
        _logger.LogInformation($"📢 Publicando evento SaleCreatedEvent para venda ID {createdSale.Id}");
        await _bus.Publish(saleEvent);

        // 🔹 9️⃣ Retornar o resultado
        return _mapper.Map<CheckoutResult>(createdSale);
    }
}
