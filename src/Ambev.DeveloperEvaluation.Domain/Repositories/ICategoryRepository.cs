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
}
