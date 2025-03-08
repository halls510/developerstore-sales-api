using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

/// <summary>
/// Handler for listing Products
/// </summary>
public class ListProductsByCategoryHandler : IRequestHandler<ListProductsByCategoryCommand, ListProductsByCategoryResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsByCategoryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ListProductsByCategoryResult> Handle(ListProductsByCategoryCommand command, CancellationToken cancellationToken)
    {       
        var validator = new ListProductsByCategoryCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var Products = await _productRepository.GetProductsByCategoryAsync(command.CategoryName,command.Page, command.Size, command.OrderBy, cancellationToken);
        var totalProducts = await _productRepository.CountProductsByCategoryAsync(command.CategoryName,cancellationToken);

        return new ListProductsByCategoryResult
        {
            Products = _mapper.Map<List<GetProductResult>>(Products),
            TotalItems = totalProducts,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
