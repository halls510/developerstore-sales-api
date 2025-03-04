using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of IProductRepository using Entity Framework Core.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of ProductRepository.
    /// </summary>
    /// <param name="context">The database context.</param>
    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new product in the database.
    /// </summary>
    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        // Se nenhuma categoria for definida, usa a categoria padrão
        if (product.CategoryId == 0)
        {
            product.CategoryId = Category.DefaultCategoryId;
        }

        // Verifica se a categoria informada existe
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }


    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a product by its title.
    /// </summary>
    public async Task<Product?> GetByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Title == title, cancellationToken);
    }

    /// <summary>
    /// Retrieves all products with pagination and sorting.
    /// </summary>
    public async Task<(IEnumerable<Product>, int, int, int)> GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();
        int totalItems = await query.CountAsync(cancellationToken);
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        if (!string.IsNullOrEmpty(orderBy))
        {
            query = orderBy.ToLower() switch
            {
                "price asc" => query.OrderBy(p => p.Price),
                "price desc" => query.OrderByDescending(p => p.Price),
                "title asc" => query.OrderBy(p => p.Title),
                "title desc" => query.OrderByDescending(p => p.Title),
                _ => query
            };
        }

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalItems, page, totalPages);
    }

    /// <summary>
    /// Updates an existing product in the database.
    /// </summary>
    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existingProduct = await GetByIdAsync(product.Id, cancellationToken);
        if (existingProduct == null)
            throw new KeyNotFoundException("Product not found.");

        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == product.CategoryId, cancellationToken);
        if (!categoryExists)
        {
            throw new KeyNotFoundException("Category not found.");
        }

        _context.Entry(existingProduct).CurrentValues.SetValues(product);
        await _context.SaveChangesAsync(cancellationToken);
        return existingProduct;
    }

    /// <summary>
    /// Deletes a product from the database.
    /// </summary>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves a paginated list of products with optional sorting.
    /// </summary>
    public async Task<List<Product>> GetProductsAsync(int page, int size, string? orderBy, CancellationToken cancellationToken)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        // Apply sorting if provided
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = ApplySorting(query, orderBy);
        }
        else
        {
            // Default sorting by ID ascending
            query = query.OrderBy(p => p.Id);
        }

        return await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Applies dynamic sorting based on a string format "field asc, field2 desc".
    /// </summary>
    private IQueryable<Product> ApplySorting(IQueryable<Product> query, string orderBy)
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
    private IQueryable<Product> ApplyOrder(IQueryable<Product> query, string propertyPath, bool isAscending)
    {
        var param = Expression.Parameter(typeof(Product), "p");
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
            new Type[] { typeof(Product), property.Type },
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<Product>(orderByExpression);
    }

    /// <summary>
    /// Retrieves the total count of products in the database.
    /// </summary>
    public async Task<int> CountProductsAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.CountAsync(cancellationToken);
    }
}