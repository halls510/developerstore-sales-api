﻿using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Cart entity operations.
/// </summary>
public interface ICartRepository
{
    /// <summary>
    /// Creates a new cart in the repository.
    /// </summary>
    /// <param name="cart">The cart to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created cart.</returns>
    Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a cart by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the cart.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The cart if found, null otherwise.</returns>
    Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all carts with optional pagination and sorting.
    /// </summary>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="pageSize">The number of items per page (default is 10).</param>
    /// <param name="orderBy">Sorting criteria, e.g., "id desc, userId asc".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of carts.</returns>
    Task<IEnumerable<Cart>> GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing cart in the repository.
    /// </summary>
    /// <param name="cart">The cart with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated cart.</returns>
    Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a cart from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the cart to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the cart was deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all carts associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of carts belonging to the specified user.</returns>
    Task<IEnumerable<Cart>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
