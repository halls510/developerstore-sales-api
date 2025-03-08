using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCarts;

/// <summary>
/// Handler for listing Products
/// </summary>
public class ListCartsHandler : IRequestHandler<ListCartsCommand, ListCartsResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ListCartsHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<ListCartsResult> Handle(ListCartsCommand command, CancellationToken cancellationToken)
    {
        var validator = new ListCartsCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var Carts = await _cartRepository.GetCartsAsync(command.Page, command.Size, command.OrderBy, command.Filters, cancellationToken);
        var totalCarts = await _cartRepository.CountCartsAsync(command.Filters, cancellationToken);

        return new ListCartsResult
        {
            Carts = _mapper.Map<List<GetCartResult>>(Carts),
            TotalItems = totalCarts,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
