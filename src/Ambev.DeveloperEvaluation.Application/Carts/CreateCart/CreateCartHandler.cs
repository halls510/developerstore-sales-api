using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

/// <summary>
/// Handler for processing CreateCartCommand requests
/// </summary>
public class CreateCartHandler : IRequestHandler<CreateCartCommand, CreateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CreateCartHandler
    /// </summary>
    public CreateCartHandler(
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
    /// Handles the CreateCartCommand request.
    /// </summary>
    public async Task<CreateCartResult> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Buscar nome do usuário
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            throw new ResourceNotFoundException("User not found", "User does not exist.");

        // Buscar produtos e validar
        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
        
        // Criar um dicionário de produtos para melhor acesso
        var productDict = existingProducts.ToDictionary(p => p.Id);

        // Verificar se há produtos que não existem
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");

        // Criar entidade Cart com nome do usuário
        var cart = _mapper.Map<Cart>(command);
        cart.UserName = $"{user.Firstname} {user.Lastname}"; // Adiciona o nome do usuário ao cart

        // Criar CartItems com nome e valor do produto
        cart.Items = command.Items.Select(item =>
        {
            var product = existingProducts.FirstOrDefault(p => p.Id == item.ProductId);
            return new CartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = product?.Title ?? "Unknown Product",
                UnitPrice = product?.Price ?? 0
            };
        }).ToList();

        // Salvar no repositório
        var createdCart = await _cartRepository.CreateAsync(cart, cancellationToken);

        // Mapear para o resultado esperado
        var result = _mapper.Map<CreateCartResult>(createdCart);
        return result;
    }
}
