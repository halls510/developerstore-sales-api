using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ICategoryRepository using Entity Framework Core.
/// </summary>
public class CategoryRepository : ICategoryRepository
{
    private readonly DefaultContext _context;

    public CategoryRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new category in the database.
    /// </summary>
    /// <param name="category">The category entity to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created category entity.</returns>
    public async Task<Category> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    /// <summary>
    /// Retrieves a category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category entity if found, null otherwise.</returns>
    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.Products) // Includes related products in the query.
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves all categories from the database, ordered by name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all categories.</returns>
    public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing category in the database.
    /// </summary>
    /// <param name="category">The category entity with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated category entity.</returns>
    /// <exception cref="KeyNotFoundException">Thrown if the category is not found.</exception>
    public async Task<Category> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        var existingCategory = await GetByIdAsync(category.Id, cancellationToken);
        if (existingCategory == null)
            throw new KeyNotFoundException("Category not found.");

        _context.Entry(existingCategory).CurrentValues.SetValues(category);
        await _context.SaveChangesAsync(cancellationToken);
        return existingCategory;
    }

    /// <summary>
    /// Deletes a category from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the category was deleted, false if not found.</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category == null)
            return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
