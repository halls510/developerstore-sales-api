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
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingSale == null)
            throw new ResourceNotFoundException("Sale not found", "Sale does not exist.");

        if (request.CustomerId != existingSale.CustomerId)
            throw new BusinessRuleException("Customer ID cannot be changed.");

        var user = await _userRepository.GetByIdAsync(existingSale.CustomerId, cancellationToken);
        if (user == null)
            throw new ResourceNotFoundException("Customer not found", "Customer does not exist.");

        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var productDict = existingProducts.ToDictionary(p => p.Id);

        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");

        var updatedItems = request.Items.Select(item =>
        {
            var product = productDict[item.ProductId];
            return new SaleItem(
                existingSale.Id,
                item.ProductId,
                product.Title,
                item.Quantity,
                product.Price
            );
        }).ToList();

        var itemsToRemove = existingSale.Items.Where(item => !productIds.Contains(item.ProductId)).ToList();
        foreach (var item in itemsToRemove)
        {
            existingSale.Items.Remove(item);
        }

        existingSale.Items = updatedItems;
        existingSale.CustomerName = $"{user.Firstname} {user.Lastname}";

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        var saleEvent = new SaleModifiedEvent(updatedSale);
        _logger.LogInformation($"Publishing SaleModifiedEvent for sale ID {updatedSale.Id}");
        await _bus.Publish(saleEvent);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }
}