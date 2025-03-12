using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

/// <summary>
/// Handler for processing UpdateCartCommand requests
/// </summary>
public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateCartHandler
    /// </summary>
    public UpdateCartHandler(
        ICartRepository cartRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateCartCommand request
    /// </summary>
    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Buscar o carrinho existente
        var existingCart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingCart == null)
            throw new ResourceNotFoundException("Cart not found", "Cart does not exist.");

        // Aplicar regra para evitar atualização de carrinhos finalizados
        OrderRules.CanCartBeUpdated(existingCart.Status, throwException: true);

        // O UserId do carrinho não pode ser alterado
        if (command.UserId != existingCart.UserId)
            throw new BusinessRuleException("User ID cannot be changed.");

        // Buscar usuário
        var user = await _userRepository.GetByIdAsync(existingCart.UserId, cancellationToken);
        if (user == null)
            throw new ResourceNotFoundException("User not found", "User does not exist.");

        // Buscar produtos e validar existência
        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var productDict = existingProducts.ToDictionary(p => p.Id);
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");

        // Criar nova lista de itens com os produtos atualizados
        var updatedItems = command.Items.Select(item =>
        {
            var product = productDict[item.ProductId];
            var existingItem = existingCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (!OrderRules.ValidateItemQuantity(item.Quantity))
                throw new BusinessRuleException($"Product {product.Title} exceeds the allowed quantity limit.");

            var discount = OrderRules.CalculateDiscount(item.Quantity, product.Price);
            var totalWithDiscount = OrderRules.CalculateTotalWithDiscount(item.Quantity, product.Price);

            return new CartItem
            {
                Id = existingItem?.Id ?? 0, // Preserve existing ID or default to 0
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = product?.Title ?? "Unknown Product",
                UnitPrice = product?.Price ?? new Money(0),
                Discount = discount,
                Total = totalWithDiscount
            };
        }).ToList();

        // Remover do banco de dados os itens que não estão na requisição
        var itemsToRemove = existingCart.Items
            .Where(item => !productIds.Contains(item.ProductId))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            existingCart.Items.Remove(item);
        }

        // Atualizar os itens e informações do carrinho
        existingCart.Items = updatedItems;
        existingCart.Date = command.Date;
        existingCart.UserName = $"{user.Firstname} {user.Lastname}";
        existingCart.TotalPrice = OrderRules.CalculateTotal(updatedItems.Select(i => (i.Quantity, i.UnitPrice)));

        // Salvar no repositório
        var updatedCart = await _cartRepository.UpdateAsync(existingCart, cancellationToken);

        // Mapear para o resultado esperado
        var result = _mapper.Map<UpdateCartResult>(updatedCart);
        return result;
    }
}
