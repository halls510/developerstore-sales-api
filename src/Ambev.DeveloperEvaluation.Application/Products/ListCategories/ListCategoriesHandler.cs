using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Products.ListCategories;

/// <summary>
/// Handler for listing Categories
/// </summary>
public class ListCategoriesHandler : IRequestHandler<ListCategoriesCommand, ListCategoriesResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ListCategoriesHandler> _logger;

    public ListCategoriesHandler(ICategoryRepository categoryRepository, IMapper mapper, ILogger<ListCategoriesHandler> logger)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ListCategoriesResult> Handle(ListCategoriesCommand command, CancellationToken cancellationToken)
    {       
        ._logger.LogInformation("Listing categories with Page: {Page}, Size: {Size}, OrderBy: {OrderBy}", command.Page, command.Size, command.OrderBy);

        var validator = new ListCategoriesCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid) {
            _logger.LogWarning("Validation failed for ListCategoriesCommand: {Errors}", validationResult.Errors);
            throw new FluentValidation.ValidationException(validationResult.Errors);
        }

        var Categories = await _categoryRepository.GetCategoriesAsync(command.Page, command.Size, command.OrderBy, cancellationToken);
        var totalCategories = await _categoryRepository.CountCategoriesAsync(cancellationToken);

        _logger.LogInformation("Successfully retrieved {TotalItems} categories", totalCategories);

        return new ListCategoriesResult
        {
            Categories = _mapper.Map<List<string>>(Categories),
            TotalItems = totalCategories,
            CurrentPage = command.Page,
            PageSize = command.Size
        };
    }
}
