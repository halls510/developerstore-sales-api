using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations.
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Checks if a sale exists by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale exists, false otherwise</returns>
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

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
    /// Retrieves a sale item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale item if found, null otherwise.</returns>
    Task<SaleItem?> GetSaleItemByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale item by its product Id
    /// </summary>
    /// <param name="productId">The product Id of the sale item.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale item if found, null otherwise.</returns>
    Task<SaleItem?> GetSaleItemByProductIdAsync(int saleId, int productId, CancellationToken cancellationToken = default);

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
    /// Updates an existing sale item in the repository.
    /// </summary>
    /// <param name="sale">The sale item with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale item.</returns>
    Task<SaleItem> UpdateItemAsync(SaleItem saleItem, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of sales with optional sorting.
    /// </summary>
    Task<List<Sale>> GetSalesAsync(int page, int size, string? orderBy, Dictionary<string, string[]>? filters, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the total count of Sales in the database.
    /// </summary>
    Task<int> CountSalesAsync(Dictionary<string, string[]>? filters, CancellationToken cancellationToken);
}
