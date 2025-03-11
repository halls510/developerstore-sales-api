using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using System.Runtime.InteropServices;

namespace Ambev.DeveloperEvaluation.Application.Carts.Checkout;


/// <summary>
/// Manipulador para processar o checkout do carrinho.
/// </summary>
public class CheckoutHandler : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly ISaleRepository _saleRepository;
    private readonly SaleHandler _saleHandler;
    private readonly IMapper _mapper;

    public CheckoutHandler(ICartRepository cartRepository, ISaleRepository saleRepository, SaleHandler saleHandler, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _saleRepository = saleRepository;
        _saleHandler = saleHandler;
        _mapper = mapper;
    }

    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        // Obtém o carrinho
        var cart = await _cartRepository.GetByIdAsync(request.CartId, cancellationToken);
        if (cart == null)
            throw new Exception("Carrinho não encontrado.");

        // Aplica regras de negócio antes do checkout
        _saleHandler.ProcessSale(cart);

        // Cria uma venda baseada no carrinho
        var sale = _mapper.Map<Sale>(cart);
        sale.SaleDate = DateTime.UtcNow;

        // Persiste a venda no banco de dados
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Remove o carrinho após a conversão
        await _cartRepository.DeleteAsync(cart.Id, cancellationToken);

        return _mapper.Map<CheckoutResult>(createdSale);
    }
}
