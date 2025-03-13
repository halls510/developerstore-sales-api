using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<CreateCartHandler> _logger;

    /// <summary>
    /// Initializes a new instance of CreateCartHandler
    /// </summary>
    public CreateCartHandler(
        ICartRepository cartRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<CreateCartHandler> logger)
    {
        _cartRepository = cartRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the CreateCartCommand request.
    /// </summary>
    public async Task<CreateCartResult> Handle(CreateCartCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando criação do carrinho para o usuário {UserId}", command.UserId);

        var validator = new CreateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando CreateCartCommand para o usuário {UserId}", command.UserId);
            throw new ValidationException(validationResult.Errors);
        }

        if (!command.Items.All(i => OrderRules.ValidateItemQuantity(i.Quantity)))
        {
            _logger.LogWarning("Quantidade inválida para um ou mais produtos no carrinho do usuário {UserId}", command.UserId);
            throw new BusinessRuleException("Quantidade inválida para um ou mais produtos.");
        }

        // Buscar nome do usuário
        _logger.LogInformation("Buscando informações do usuário {UserId}", command.UserId);
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Usuário {UserId} não encontrado", command.UserId);
            throw new ResourceNotFoundException("User not found", "User does not exist.");
        }

        // Buscar produtos e validar
        _logger.LogInformation("Buscando informações dos produtos no carrinho do usuário {UserId}", command.UserId);
        var productIds = command.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        // Criar um dicionário de produtos para melhor acesso
        var productDict = existingProducts.ToDictionary(p => p.Id);

        // Verificar se há produtos que não existem
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
        {
            _logger.LogWarning("Produtos não encontrados para o usuário {UserId}: {MissingProducts}", command.UserId, string.Join(", ", missingProducts));
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");
        }

        // Criar entidade Cart com nome do usuário
        var cart = _mapper.Map<Cart>(command);
        cart.UserName = $"{user.Firstname} {user.Lastname}";
        

        _logger.LogInformation("Criando itens do carrinho para o usuário {UserId}", command.UserId);

        // Criar CartItems 
        cart.Items = command.Items.Select(item =>
        {
            var product = existingProducts.FirstOrDefault(p => p.Id == item.ProductId);
            var price = product?.Price ?? new Money(0);
            var discount = OrderRules.CalculateDiscount(item.Quantity, price);
            var totalWithDiscount = OrderRules.CalculateTotalWithDiscount(item.Quantity, price);

            return new CartItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                ProductName = product?.Title ?? "Unknown Product",
                UnitPrice = price,
                Discount = discount,
                Total = totalWithDiscount
            };
        }).ToList();

        cart.TotalPrice = OrderRules.CalculateTotal(cart.Items.Select(i => (i.Quantity, i.UnitPrice)));

        // Salvar no repositório
        _logger.LogInformation("Salvando o carrinho no banco de dados para o usuário {UserId}", command.UserId);
        var createdCart = await _cartRepository.CreateAsync(cart, cancellationToken);

        _logger.LogInformation("Carrinho {CartId} criado com sucesso para o usuário {UserId}", createdCart.Id, command.UserId);

        // Mapear para o resultado esperado
        var result = _mapper.Map<CreateCartResult>(createdCart);

        _logger.LogInformation("Finalizando criação do carrinho {CartId} para o usuário {UserId}", createdCart.Id, command.UserId);
        return result;
    }
}
