using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Repositories;

public class CategoryRepositoryTests
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly CancellationToken _cancellationToken;

    public CategoryRepositoryTests()
    {
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _cancellationToken = new CancellationToken();
    }

    [Fact(DisplayName = "CreateAsync should return created category")]
    public async Task Given_NewCategory_When_CreateAsyncCalled_Then_ShouldReturnCreatedCategory()
    {
        // Arrange
        var newCategory = new Category { Id = 1, Name = "Electronics" };
        _categoryRepository.CreateAsync(newCategory, _cancellationToken).Returns(newCategory);

        // Act
        var result = await _categoryRepository.CreateAsync(newCategory, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(newCategory);
    }

    [Fact(DisplayName = "GetByIdAsync should return category when id exists")]
    public async Task Given_ExistingCategoryId_When_GetByIdAsyncCalled_Then_ShouldReturnCategory()
    {
        // Arrange
        var expectedCategory = new Category { Id = 1, Name = "Electronics" };
        _categoryRepository.GetByIdAsync(1, _cancellationToken).Returns(expectedCategory);

        // Act
        var result = await _categoryRepository.GetByIdAsync(1, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedCategory);
    }

    [Fact(DisplayName = "GetByNameAsync should return category when name exists")]
    public async Task Given_ExistingCategoryName_When_GetByNameAsyncCalled_Then_ShouldReturnCategory()
    {
        // Arrange
        var expectedCategory = new Category { Id = 1, Name = "Electronics" };
        _categoryRepository.GetByNameAsync("Electronics", _cancellationToken).Returns(expectedCategory);

        // Act
        var result = await _categoryRepository.GetByNameAsync("Electronics", _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedCategory);
    }

    [Fact(DisplayName = "DeleteAsync should return true when category is deleted")]
    public async Task Given_ExistingCategoryId_When_DeleteAsyncCalled_Then_ShouldReturnTrue()
    {
        // Arrange
        _categoryRepository.DeleteAsync(1, _cancellationToken).Returns(true);

        // Act
        var result = await _categoryRepository.DeleteAsync(1, _cancellationToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "GetCategoriesAsync should return list of categories")]
    public async Task Given_Pagination_When_GetCategoriesAsyncCalled_Then_ShouldReturnCategoryList()
    {
        // Arrange
        var categories = new List<Category> { new Category { Id = 1, Name = "Electronics" } };
        _categoryRepository.GetCategoriesAsync(1, 10, null, _cancellationToken).Returns(categories);

        // Act
        var result = await _categoryRepository.GetCategoriesAsync(1, 10, null, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(categories);
    }
}