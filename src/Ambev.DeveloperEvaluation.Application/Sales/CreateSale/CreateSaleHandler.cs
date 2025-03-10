using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Rebus.Bus;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(ISaleRepository saleRepository,
        IUserRepository userRepository,
        IProductRepository productRepository,
        IMapper mapper,
        IBus bus,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _bus = bus;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        // 📌 1️⃣ Validar o comando com FluentValidation
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // 📌 2️⃣ Buscar usuário e validar existência
        var user = await _userRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (user == null)
            throw new ResourceNotFoundException("Customer not found", $"Customer with ID {request.CustomerId} does not exist.");

        // 📌 3️⃣ Buscar produtos e validar existência
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var existingProducts = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        // Criar um dicionário de produtos para melhor acesso
        var productDict = existingProducts.ToDictionary(p => p.Id);

        // Verificar se há produtos que não existem
        var missingProducts = productIds.Except(productDict.Keys).ToList();
        if (missingProducts.Any())
            throw new ResourceNotFoundException("Product not found", $"The following product(s) do not exist: {string.Join(", ", missingProducts)}");

        // 📌 4️⃣ Criar entidade Sale com nome do cliente
        var sale = _mapper.Map<Sale>(request);
        sale.CustomerName = $"{user.Firstname} {user.Lastname}"; // Adiciona nome do usuário à venda

        // 📌 5️⃣ Criar SaleItems com nome e preço do produto
        sale.Items = request.Items.Select(item =>
        {
            var product = productDict[item.ProductId];
            return new SaleItem(
                sale.Id,
                item.ProductId,
                product.Title,
                item.Quantity,
                product.Price
            );
        }).ToList();

        // 📌 6️⃣ Salvar a venda no repositório
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Publicar evento de Venda Criada
        var saleEvent = new SaleCreatedEvent(createdSale);
        // LOG para verificar se o evento está sendo publicado
        _logger.LogInformation($"🟢 Publicando evento SaleCreatedEvent para venda ID {createdSale.Id}");
        await _bus.Publish(saleEvent);

        // 📌 8️⃣ Retornar Resultado da Venda
        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}
