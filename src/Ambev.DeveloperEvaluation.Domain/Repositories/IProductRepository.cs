using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Product entity operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Creates a new product in the repository.
    /// </summary>
    /// <param name="product">The product to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created product.</returns>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a list of products exist by their IDs.
    /// </summary>
    /// <param name="productIds">List of product IDs.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of existing products.</returns>
    Task<List<Product>> GetByIdsAsync(List<int> productIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, null otherwise.</returns>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product by its title
    /// </summary>
    /// <param name="title">The title of the product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, null otherwise.</returns>
    Task<Product?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all products with pagination and sorting.
    /// </summary>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="orderBy">Sorting criteria, e.g., "price desc, title asc".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A tuple containing:
    /// - A collection of products.
    /// </returns>
    Task<List<Product>>  GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product in the repository.
    /// </summary>
    /// <param name="product">The product with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated product.</returns>
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the product was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of products with optional sorting.
    /// </summary>
    /// <param name="page">The page number for pagination (starting from 1).</param>
    /// <param name="size">The number of products per page.</param>
    /// <param name="orderBy">
    /// Sorting criteria in the format "field asc" or "field desc".
    /// Multiple fields can be separated by commas (e.g., "title asc, price desc").
    /// </param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation, containing a paginated list of products.</returns>
    Task<List<Product>> GetProductsAsync(int page, int size, string? orderBy, Dictionary<string, string[]>? filters, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the total count of products available in the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A task representing the asynchronous operation, containing the total number of products.</returns>
    Task<int> CountProductsAsync(Dictionary<string, string[]>? filters, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of products filtered by category with optional sorting.
    /// </summary>
    Task<List<Product>> GetProductsByCategoryAsync(string categoryName, int page, int size, string? orderBy, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the total count of products filtered by category.
    /// </summary>
    Task<int> CountProductsByCategoryAsync(string categoryName, CancellationToken cancellationToken);
}
