using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

/// <summary>
/// Handler for listing Products
/// </summary>
public class ListProductsHandler : IRequestHandler<ListProductsCommand, ListProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ListProductsResult> Handle(ListProductsCommand command, CancellationToken cancellationToken)
    {       
        var validator = new ListProductsCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var Products = await _productRepository.GetProductsAsync(command.Page, command.Size, command.OrderBy, cancellationToken);
        var totalProducts = await _productRepository.CountProductsAsync(cancellationToken);

        return new ListProductsResult
        {
            Products = _mapper.Map<List<GetProductResult>>(Products),
            TotalItems = totalProducts,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
