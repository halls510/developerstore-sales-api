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
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product if found, null otherwise.</returns>
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

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
    /// - Total number of items.
    /// - Current page.
    /// - Total number of pages.
    /// </returns>
    Task<(IEnumerable<Product> Products, int TotalItems, int CurrentPage, int TotalPages)>
        GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default);

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
    /// Retrieves all product categories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of product categories.</returns>
    Task<IEnumerable<string>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves products by category with pagination and sorting.
    /// </summary>
    /// <param name="categoryName">The name of the category.</param>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="orderBy">Sorting criteria, e.g., "price desc, title asc".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A tuple containing:
    /// - A collection of products within the specified category.
    /// - Total number of items.
    /// - Current page.
    /// - Total number of pages.
    /// </returns>
    Task<(IEnumerable<Product> Products, int TotalItems, int CurrentPage, int TotalPages)>
        GetByCategoryAsync(string categoryName, int page, int pageSize, string orderBy, CancellationToken cancellationToken = default);
}
