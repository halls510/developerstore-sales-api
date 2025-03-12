using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IMapper mapper,
        IBus bus,
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _bus = bus;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização da venda {SaleId}", request.Id);

        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando UpdateSaleCommand para a venda {SaleId}", request.Id);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando venda {SaleId} no banco de dados", request.Id);
        var existingSale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingSale == null)
        {
            _logger.LogWarning("Venda {SaleId} não encontrada", request.Id);
            throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");
        }

        if (request.CustomerId != existingSale.CustomerId)
        {
            _logger.LogWarning("Tentativa de alteração do CustomerId na venda {SaleId}", request.Id);
            throw new BusinessRuleException("Customer ID cannot be changed.");
        }

        _logger.LogInformation("Buscando cliente {CustomerId} associado à venda {SaleId}", existingSale.CustomerId, request.Id);
        var user = await _userRepository.GetByIdAsync(existingSale.CustomerId, cancellationToken);
        if (user == null)
        {
            _logger.LogWarning("Cliente {CustomerId} não encontrado", existingSale.CustomerId);
            throw new ResourceNotFoundException("Customer not found", "Customer does not exist.");
        }

        _logger.LogInformation("Buscando produtos associados à venda {SaleId}", request.Id);
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var productDict = existingProducts.ToDictionary(p => p.Id);
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
        {
            _logger.LogWarning("Produtos não encontrados na venda {SaleId}: {MissingProducts}", request.Id, string.Join(", ", missingProducts));
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");
        }

        _logger.LogInformation("Atualizando itens da venda {SaleId}", request.Id);
        var updatedItems = request.Items.Select(item =>
        {
            var product = productDict[item.ProductId];
            var existingItem = existingSale.Items.FirstOrDefault(i => i.ProductId == item.ProductId);

            if (!OrderRules.ValidateItemQuantity(item.Quantity))
            {
                _logger.LogWarning("Produto {ProductId} na venda {SaleId} excede a quantidade permitida", item.ProductId, request.Id);
                throw new BusinessRuleException($"Product {product.Title} exceeds the allowed quantity limit.");
            }

            var discount = OrderRules.CalculateDiscount(item.Quantity, product.Price);
            var totalWithDiscount = OrderRules.CalculateTotalWithDiscount(item.Quantity, product.Price);

            return new SaleItem(
                existingItem?.Id ?? 0, // Preserve existing ID or default to 0
                existingSale.Id,
                product.Id,
                product.Title,
                item.Quantity,
                product.Price,
                discount,
                totalWithDiscount
            );
        }).ToList();

        existingSale.Items = updatedItems;
        existingSale.CustomerName = $"{user.Firstname} {user.Lastname}";
        existingSale.TotalValue = new Money(updatedItems.Sum(i => i.Total.Amount));

        _logger.LogInformation("Salvando venda {SaleId} no banco de dados", request.Id);
        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        _logger.LogInformation("Venda {SaleId} atualizada com sucesso", request.Id);

        var saleEvent = new SaleModifiedEvent(updatedSale);
        _logger.LogInformation("📢 Publicando evento SaleModifiedEvent para venda ID {SaleId}", updatedSale.Id);
        await _bus.Publish(saleEvent);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
