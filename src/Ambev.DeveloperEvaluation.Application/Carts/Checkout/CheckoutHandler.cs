using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using MediatR;
using Rebus.Bus;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Application.Common.Messaging;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;

/// <summary>
/// Manipulador para processar o checkout do carrinho.
/// </summary>
public class CheckoutHandler : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly IRabbitMqPublisher _rabbitMqPublisher;
    private readonly IMapper _mapper;
    private readonly ILogger<CheckoutHandler> _logger;

    public CheckoutHandler(
        ICartRepository cartRepository, 
        ISaleRepository saleRepository,
        IRabbitMqPublisher rabbitMqPublisher, 
        IMapper mapper, 
        ILogger<CheckoutHandler> logger)
    {
        _cartRepository = cartRepository;
        _saleRepository = saleRepository;
        _rabbitMqPublisher = rabbitMqPublisher;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando checkout para o carrinho {CartId}", request.CartId);

        // 🔹 1️⃣ Obter o carrinho
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
        {
            _logger.LogWarning("Carrinho {CartId} não encontrado.", request.CartId);
            throw new ResourceNotFoundException("Carrinho não encontrado.", $"Carrinho {request.CartId} não encontrado.");
        }

        // 🔹 2️⃣ Verificar se o carrinho pode ser finalizado
        if (cart.Status != CartStatus.Active)
        {
            _logger.LogWarning("Carrinho {CartId} não pode ser finalizado pois não está ativo.", request.CartId);
            throw new Exception("O carrinho não pode ser finalizado pois não está ativo.");
        }

        var items = cart.Items.Select(i => (i.Quantity, i.UnitPrice)).ToList();

        // 🔹 3️⃣ Aplicar regras de negócio
        _logger.LogInformation("Validando regras de negócio para o checkout do carrinho {CartId}", request.CartId);
        OrderRules.ValidateCartForCheckout(items);

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
        _logger.LogInformation("Total calculado para a venda: {TotalValue}", sale.TotalValue);

        // 🔹 6️⃣ Persistir a venda no banco de dados
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        _logger.LogInformation("Venda {SaleId} criada com sucesso para o usuário {UserId}", createdSale.Id, createdSale.CustomerId);

        // 🔹 7️⃣ Atualizar o status do carrinho para "CheckedOut"
        cart.MarkAsCheckedOut();
        await _cartRepository.UpdateAsync(cart, cancellationToken);
        _logger.LogInformation("Carrinho {CartId} atualizado para CheckedOut.", request.CartId);

        // 🔹 8️⃣ Publicar o evento de venda criada
        var saleEvent = new SaleCreatedEvent(createdSale);
        _logger.LogInformation("Publicando evento SaleCreatedEvent para venda ID {SaleId}", createdSale.Id);
        await _rabbitMqPublisher.SendAsync(saleEvent);

        // 🔹 9️⃣ Retornar o resultado
        _logger.LogInformation("Checkout finalizado com sucesso para o carrinho {CartId}.", request.CartId);
        return _mapper.Map<CheckoutResult>(createdSale);
    }
}
