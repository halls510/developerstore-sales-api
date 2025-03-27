using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<UpdateCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of UpdateCartHandler
    /// </summary>
    public UpdateCartHandler(
        ICartRepository cartRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<UpdateCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the UpdateCartCommand request
    /// </summary>
    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização do carrinho {CartId}", command.Id);

        var validator = new UpdateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando UpdateCartCommand para o carrinho {CartId}", command.Id);
            throw new ValidationException(validationResult.Errors);
        }

        // Buscar o carrinho existente
        _logger.LogInformation("Buscando carrinho {CartId} no banco de dados", command.Id);
        var existingCart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existingCart == null)
        {
            _logger.LogWarning("Carrinho {CartId} não encontrado", command.Id);
            throw new ResourceNotFoundException("Cart not found", "Cart does not exist.");
        }

        // Aplicar regra para evitar atualização de carrinhos finalizados
        _logger.LogInformation("Validando se o carrinho {CartId} pode ser atualizado", command.Id);
        OrderRules.CanCartBeUpdated(existingCart.Status, throwException: true);

        // O UserId do carrinho não pode ser alterado
        if (command.UserId != existingCart.UserId)
        {
            _logger.LogWarning("Tentativa de alteração do UserId para o carrinho {CartId}", command.Id);
            throw new BusinessRuleException("User ID cannot be changed.");
        }

        // Buscar usuário
        _logger.LogInformation("Buscando usuário {UserId} associado ao carrinho {CartId}", existingCart.UserId, command.Id);
        var user = await _userRepository.GetByIdAsync(existingCart.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", existingCart.UserId);
            throw new ResourceNotFoundException("User not found", "User does not exist.");
        }

        // Buscar produtos e validar existência
        _logger.LogInformation("Buscando produtos associados ao carrinho {CartId}", command.Id);
        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var productDict = existingProducts.ToDictionary(p => p.Id);
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
        {
            _logger.LogWarning("Produtos não encontrados no carrinho {CartId}: {MissingProducts}", command.Id, string.Join(", ", missingProducts));
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");
        }

        // Criar nova lista de itens com os produtos atualizados
        var updatedItems = command.Items.Select(item =>
        {
            var product = productDict[item.ProductId];
            var existingItem = existingCart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (!OrderRules.ValidateItemQuantity(item.Quantity))
            {
                _logger.LogWarning("Produto {ProductId} no carrinho {CartId} excede a quantidade permitida", item.ProductId, command.Id);
                throw new BusinessRuleException($"Product {product.Title} exceeds the allowed quantity limit.");
            }

            var discount = OrderRules.CalculateDiscount(item.Quantity, product.Price);
            var totalWithDiscount = OrderRules.CalculateTotalWithDiscount(item.Quantity, product.Price);

            return new CartItem
            {
                Id = existingItem?.Id ?? 0, // Preserve existing ID or default to 0
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = product?.Title ?? "Unknown Product",
                UnitPrice = new Money(product?.Price ?? new Money(0)),
                Discount = new Money(discount),
                Total = new Money(totalWithDiscount)
            };
        }).ToList();

        // Remover do banco de dados os itens que não estão na requisição
        var itemsToRemove = existingCart.Items
            .Where(item => !productIds.Contains(item.ProductId))
            .ToList();

        foreach (var item in itemsToRemove)
        {
            _logger.LogInformation("Removendo item {ProductId} do carrinho {CartId}", item.ProductId, command.Id);
            existingCart.Items.Remove(item);
        }

        // Atualizar os itens e informações do carrinho
        existingCart.Items = updatedItems;
        existingCart.Date = command.Date;
        existingCart.UserName = $"{user.Firstname} {user.Lastname}";
        existingCart.TotalPrice = OrderRules.CalculateTotal(updatedItems.Select(i => (i.Quantity, i.UnitPrice)));

        // Salvar no repositório
        _logger.LogInformation("Salvando alterações no carrinho {CartId} no banco de dados", command.Id);
        var updatedCart = await _cartRepository.UpdateAsync(existingCart, cancellationToken);

        _logger.LogInformation("Carrinho {CartId} atualizado com sucesso", command.Id);

        // Mapear para o resultado esperado
        var result = _mapper.Map<UpdateCartResult>(updatedCart);
        return result;
    }
}
