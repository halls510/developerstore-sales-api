using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found, null otherwise.</returns>
    Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales with optional pagination and sorting.
    /// </summary>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="orderBy">Sorting criteria, e.g., "date desc, totalValue asc".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of sales.</returns>
    Task<IEnumerable<Sale>> GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository.
    /// </summary>
    /// <param name="sale">The sale with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale.</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
