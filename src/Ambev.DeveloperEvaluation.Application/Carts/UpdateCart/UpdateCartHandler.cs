using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

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
            throw new KeyNotFoundException("Cart does not exist.");

        // O UserId do carrinho não pode ser alterado
        if (command.UserId != existingCart.UserId)
            throw new InvalidOperationException("User ID cannot be changed.");

        // Buscar usuário
        var user = await _userRepository.GetByIdAsync(existingCart.UserId, cancellationToken);
        if (user == null)
            throw new KeyNotFoundException("User does not exist.");

        // Buscar produtos e validar
        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        // Criar um dicionário de produtos para acesso rápido
        var productDict = existingProducts.ToDictionary(p => p.Id);

        // Verificar se há produtos que não existem no banco de dados
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
            throw new KeyNotFoundException($"The following product(s) do not exist: {string.Join(", ", missingProducts)}");

        // Criar nova lista de itens com os produtos enviados na requisição
        var updatedItems = command.Items.Select(item =>
        {
            var product = productDict[item.ProductId];
            return new CartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = product.Title,
                UnitPrice = product.Price
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

        // Atualizar os itens do carrinho
        existingCart.Items = updatedItems;

        // Atualizar informações básicas do carrinho
        existingCart.Date = command.Date;
        existingCart.UserName = $"{user.Firstname} {user.Lastname}";

        // Salvar no repositório
        var updatedCart = await _cartRepository.UpdateAsync(existingCart, cancellationToken);

        // Mapear para o resultado esperado
        var result = _mapper.Map<UpdateCartResult>(updatedCart);
        return result;
    }
}
