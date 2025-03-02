using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
        if (product.CategoryId == Guid.Empty)
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
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
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
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves all unique product categories.
    /// </summary>
    public async Task<IEnumerable<string>> GetAllCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Select(c => c.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves products by category name with pagination and sorting.
    /// </summary>
    public async Task<(IEnumerable<Product>, int, int, int)> GetByCategoryAsync(string categoryName, int page, int pageSize, string orderBy, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName, cancellationToken);

        if (category == null)
        {
            return (Enumerable.Empty<Product>(), 0, 1, 1);
        }

        var query = _context.Products
            .Where(p => p.CategoryId == category.Id)
            .Include(p => p.Category)
            .AsQueryable();

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
}