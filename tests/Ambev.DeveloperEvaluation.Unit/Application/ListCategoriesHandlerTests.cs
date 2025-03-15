using Ambev.DeveloperEvaluation.Application.Products.ListCategories;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class ListCategoriesHandlerTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;
    private readonly ListCategoriesHandler _handler;
    private readonly ILogger<ListCategoriesHandler> _logger;

    public ListCategoriesHandlerTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ListCategoriesHandler>>();
        _handler = new ListCategoriesHandler(_categoryRepository, _mapper, _logger);
    }

    [Fact(DisplayName = "Given a valid command, When listing categories, Then returns a paginated category list")]
    public async Task Handle_ValidRequest_ReturnsCategoryList()
    {
        var command = ListCategoriesHandlerTestData.GenerateValidCommand();
        var categories = ListCategoriesHandlerTestData.GenerateCategoryList();

        _categoryRepository.GetCategoriesAsync(command.Page, command.Size, command.OrderBy, Arg.Any<CancellationToken>())
            .Returns(categories);
        _categoryRepository.CountCategoriesAsync(Arg.Any<CancellationToken>()).Returns(categories.Count);
        _mapper.Map<List<string>>(categories).Returns(categories.ConvertAll(c => c.Name));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Categories.Should().NotBeNull();
        result.TotalItems.Should().Be(categories.Count);
        result.CurrentPage.Should().Be(command.Page);
        result.PageSize.Should().Be(command.Size);
    }

    [Fact(DisplayName = "Given an invalid command, When validation fails, Then throws ValidationException")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        var invalidCommand = new ListCategoriesCommand { Page = -1, Size = 0, OrderBy = "" };

        var act = async () => await _handler.Handle(invalidCommand, CancellationToken.None);

        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}