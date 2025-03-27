using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Category entity operations.
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Creates a new category in the repository.
    /// </summary>
    /// <param name="category">The category entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category entity.</returns>
    Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category entity if found, null otherwise.</returns>
    Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a category by its name
    /// </summary>
    /// <param name="name">The name of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category entity if found, null otherwise.</returns
    /// <returns></returns>
    Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all categories from the repository, ordered by name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of all categories.</returns>
    Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing category in the repository.
    /// </summary>
    /// <param name="category">The category entity with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated category entity.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the category is not found.</exception>
    Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a category from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the category was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of categories with optional sorting.
    /// </summary>
    /// <param name="page">The page number for pagination (starting from 1).</param>
    /// <param name="size">The number of categories per page.</param>
    /// <param name="orderBy">
    /// Sorting criteria in the format "field asc" or "field desc".
    /// Multiple fields can be separated by commas (e.g., "name asc, id desc").
    /// </param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated list of categories.</returns>
    Task<List<Category>> GetCategoriesAsync(int page, int size, string? orderBy, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the total count of categories available in the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation, containing the total number of categories.</returns>
    Task<int> CountCategoriesAsync(CancellationToken cancellationToken);
}
