using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
    /// Retrieves a category by its name.
    /// </summary>
    /// <param name="name">The name of the category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The category entity if found, null otherwise.</returns>
    public async Task<Category?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
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
            throw new ResourceNotFoundException("Category not found", $"Category with ID {category.Id} not found.");

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

    /// <summary>
    /// Retrieves a paginated list of categories with optional sorting.
    /// </summary>
    public async Task<List<Category>> GetCategoriesAsync(int page, int size, string? orderBy, CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsQueryable();

        // Apply sorting if provided
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = ApplySorting(query, orderBy);
        }
        else
        {
            // Default sorting by Name ascending
            query = query.OrderBy(c => c.Name);
        }

        return await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Applies dynamic sorting based on a string format "field asc, field2 desc".
    /// </summary>
    private IQueryable<Category> ApplySorting(IQueryable<Category> query, string orderBy)
    {
        var orderingParams = orderBy.Split(',')
            .Select(o => o.Trim().Split(' '))
            .Where(o => o.Length > 0)
            .Select(o => new { Field = o[0], IsAscending = o.Length < 2 || o[1].ToLower() == "asc" });

        foreach (var param in orderingParams)
        {
            query = ApplyOrder(query, param.Field, param.IsAscending);
        }

        return query;
    }

    /// <summary>
    /// Applies generic sorting to an IQueryable, including nested properties.
    /// </summary>
    private IQueryable<Category> ApplyOrder(IQueryable<Category> query, string propertyPath, bool isAscending)
    {
        var param = Expression.Parameter(typeof(Category), "c");
        Expression property = param;

        foreach (var prop in propertyPath.Split('.'))
        {
            property = Expression.Property(property, prop);
        }

        var lambda = Expression.Lambda(property, param);
        string methodName = isAscending ? "OrderBy" : "OrderByDescending";

        var orderByExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(Category), property.Type },
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<Category>(orderByExpression);
    }

    /// <summary>
    /// Retrieves the total count of categories in the database.
    /// </summary>
    public async Task<int> CountCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories.CountAsync(cancellationToken);
    }

    private Dictionary<string, string[]> CleanFilters(Dictionary<string, string[]>? filters)
    {
        if (filters == null) return new Dictionary<string, string[]>();

        var cleanedFilters = filters
            .Where(kvp => kvp.Key != "_page" && kvp.Key != "_size" && kvp.Key != "_order")
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return cleanedFilters;
    }
}
