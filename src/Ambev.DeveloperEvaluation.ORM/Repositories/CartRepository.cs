using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ICartRepository using Entity Framework Core.
/// </summary>
public class CartRepository : ICartRepository
{
    private readonly DefaultContext _context;

    public CartRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Carts.AddAsync(cart, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity; // Retorna o objeto com ID gerado
    }

    public async Task<Cart?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Cart>> GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default)
    {
        var query = _context.Carts.Include(c => c.Items).AsQueryable();

        if (!string.IsNullOrEmpty(orderBy))
        {
            query = orderBy.ToLower() switch
            {
                "id asc" => query.OrderBy(c => c.Id),
                "id desc" => query.OrderByDescending(c => c.Id),
                "userid asc" => query.OrderBy(c => c.UserId),
                "userid desc" => query.OrderByDescending(c => c.UserId),
                _ => query
            };
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        var existingCart = await GetByIdAsync(cart.Id, cancellationToken);
        if (existingCart == null)
            throw new KeyNotFoundException("Cart not found.");

        _context.Entry(existingCart).CurrentValues.SetValues(cart);
        existingCart.Items = cart.Items;
        await _context.SaveChangesAsync(cancellationToken);
        return existingCart;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var cart = await GetByIdAsync(id, cancellationToken);
        if (cart == null)
            return false;

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves all carts associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of carts belonging to the specified user.</returns>
    public async Task<IEnumerable<Cart>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.Items)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of carts with optional sorting.
    /// </summary>
    public async Task<List<Cart>> GetCartsAsync(int page, int size, string? orderBy, Dictionary<string, string[]>? filters, CancellationToken cancellationToken)
    {
        var query = _context.Carts.Include(p => p.Items).AsQueryable();

        // Aplica filtros
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(BuildPredicate<Cart>(filter.Key, filter.Value));
            }
        }

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
    private IQueryable<Cart> ApplySorting(IQueryable<Cart> query, string orderBy)
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
    private IQueryable<Cart> ApplyOrder(IQueryable<Cart> query, string propertyPath, bool isAscending)
    {
        var param = Expression.Parameter(typeof(Cart), "p");
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
            new Type[] { typeof(Cart), property.Type },
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<Cart>(orderByExpression);
    }

    /// <summary>
    /// Retrieves the total count of Carts in the database.
    /// </summary>
    public async Task<int> CountCartsAsync(Dictionary<string, string[]>? filters, CancellationToken cancellationToken)
    {
        //return await _context.Carts.CountAsync(cancellationToken);
        var query = _context.Carts.AsQueryable();

        // Aplica filtros
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(BuildPredicate<Cart>(filter.Key, filter.Value));
            }
        }

        return await query.CountAsync(cancellationToken);
    }

    private static Expression<Func<T, bool>> BuildPredicate<T>(string property, string[] values)
    {
        var param = Expression.Parameter(typeof(T), "x");

        bool isMin = property.StartsWith("_min");
        bool isMax = property.StartsWith("_max");
        string propertyName = isMin || isMax ? property.Substring(4) : property;

        // Divide as propriedades aninhadas (ex: "Category.Name")
        Expression prop = param;
        foreach (var propPart in propertyName.Split('.'))
        {
            prop = Expression.Property(prop, propPart);
        }

        Expression? body = null;

        foreach (var val in values)
        {
            var trimmedValue = val.Trim('*');
            var convertedValue = Convert.ChangeType(trimmedValue, prop.Type);
            var constant = Expression.Constant(convertedValue);

            Expression condition;
            if (val.StartsWith("*") && val.EndsWith("*")) // Contém
                condition = Expression.Call(prop, "Contains", Type.EmptyTypes, constant);
            else if (val.StartsWith("*")) // Termina com
                condition = Expression.Call(prop, "EndsWith", Type.EmptyTypes, constant);
            else if (val.EndsWith("*")) // Começa com
                condition = Expression.Call(prop, "StartsWith", Type.EmptyTypes, constant);
            else if (isMin) // Valor mínimo
                condition = Expression.GreaterThanOrEqual(prop, constant);
            else if (isMax) // Valor máximo
                condition = Expression.LessThanOrEqual(prop, constant);
            else // Igualdade normal
                condition = Expression.Equal(prop, constant);

            // Se houver múltiplos valores no mesmo campo, aplicar OR entre eles
            body = body == null ? condition : Expression.OrElse(body, condition);
        }

        return Expression.Lambda<Func<T, bool>>(body ?? Expression.Constant(true), param);
    }
}
