using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.BusinessRules;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCarts;

/// <summary>
/// Handler for listing Products
/// </summary>
public class ListCartsHandler : IRequestHandler<ListCartsCommand, ListCartsResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListCartsHandler> _logger;

    public ListCartsHandler(ICartRepository cartRepository, IMapper mapper, ILogger<ListCartsHandler> logger)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ListCartsResult> Handle(ListCartsCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando listagem de carrinhos - Página {Page}, Tamanho {Size}, Ordenação {OrderBy}",
            command.Page, command.Size, command.OrderBy ?? "default");

        var validator = new ListCartsCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Falha na validação do comando ListCartsCommand");
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("Buscando carrinhos do banco de dados...");
        var carts = await _cartRepository.GetCartsAsync(command.Page, command.Size, command.OrderBy, command.Filters, cancellationToken);
        var totalCarts = await _cartRepository.CountCartsAsync(command.Filters, cancellationToken);

        _logger.LogInformation("Aplicando regras de negócio para filtragem de carrinhos cancelados...");
        carts = carts.Where(cart => OrderRules.CanCartBeRetrieved(cart.Status, throwException: false)).ToList();

        _logger.LogInformation("Listagem de carrinhos concluída com {TotalCarts} carrinhos encontrados", carts.Count);

        var result = new ListCartsResult
        {
            Carts = _mapper.Map<List<GetCartResult>>(carts),
            TotalItems = totalCarts,
            CurrentPage = command.Page,
            PageSize = command.Size
        };

        return result;
    }
}
