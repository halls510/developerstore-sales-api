using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

/// <summary>
/// Handler for listing Categories
/// </summary>
public class ListCategoriesHandler : IRequestHandler<ListCategoriesCommand, ListCategoriesResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public ListCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<ListCategoriesResult> Handle(ListCategoriesCommand command, CancellationToken cancellationToken)
    {       
        var validator = new ListCategoriesCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new FluentValidation.ValidationException(validationResult.Errors);

        var Categories = await _categoryRepository.GetCategoriesAsync(command.Page, command.Size, command.OrderBy, cancellationToken);
        var totalCategories = await _categoryRepository.CountCategoriesAsync(cancellationToken);

        return new ListCategoriesResult
        {
            Categories = _mapper.Map<List<string>>(Categories),
            TotalItems = totalCategories,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
