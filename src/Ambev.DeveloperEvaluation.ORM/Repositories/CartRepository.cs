using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
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
            throw new ResourceNotFoundException("Cart not found", $"Cart with ID {cart.Id} not found.");

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
        var query = _context.Carts.Include(p => p.Items).AsQueryable();

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

        // Converte o nome do filtro para minúsculas
        propertyName = propertyName.ToLower();

        // Substituir "products" por "items" no nome do filtro
        propertyName = propertyName.Replace("products", "items");

        // Substitui "." por "_" para aceitar os dois formatos
        propertyName = propertyName.Replace(".", "_");

        // Divide propriedades aninhadas (ex: "products_productid" ou "items.productid")
        var properties = propertyName.Split('_');

        Expression prop = param;
        Type currentType = typeof(T);

        for (int i = 0; i < properties.Length; i++)
        {
            string propPart = properties[i];

            // Encontra a propriedade correspondente ignorando case (case-insensitive)
            var propInfo = currentType.GetProperties()
                                      .FirstOrDefault(p => p.Name.ToLower() == propPart);

            if (propInfo == null)
            {
                throw new InvalidOperationException($"Propriedade '{propPart}' não encontrada em {currentType.Name}");
            }

            // **Se a propriedade é uma coleção (List<T>), preparamos para Any()**
            if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = propInfo.PropertyType.GetGenericArguments().First(); // Tipo dos elementos da coleção (CartItem)
                var collectionParam = Expression.Parameter(elementType, "i");

                // **Atualiza currentType para acessar corretamente os campos do item da coleção (CartItem)**
                currentType = elementType;

                // **Verifica se há mais propriedades para acessar dentro da coleção**
                if (i + 1 < properties.Length)
                {
                    string nestedProperty = properties[i + 1]; // Exemplo: "productid"

                    // Encontra a propriedade ignorando case (case-insensitive)
                    var nestedPropInfo = currentType.GetProperties()
                                                    .FirstOrDefault(p => p.Name.ToLower() == nestedProperty);

                    if (nestedPropInfo == null)
                    {
                        throw new InvalidOperationException($"Propriedade '{nestedProperty}' não encontrada em {currentType.Name}");
                    }

                    var collectionProp = Expression.Property(collectionParam, nestedPropInfo.Name);

                    Expression? body = null;

                    // **Agora processamos corretamente os valores dentro da coleção**
                    foreach (var val in values)
                    {
                        var trimmedValue = val.Trim('*');
                        var convertedValue = Convert.ChangeType(trimmedValue, collectionProp.Type);
                        var constant = Expression.Constant(convertedValue);

                        Expression condition;
                        if (val.StartsWith("*") && val.EndsWith("*")) // Contém
                            condition = Expression.Call(collectionProp, "Contains", Type.EmptyTypes, constant);
                        else if (val.StartsWith("*")) // Termina com
                            condition = Expression.Call(collectionProp, "EndsWith", Type.EmptyTypes, constant);
                        else if (val.EndsWith("*")) // Começa com
                            condition = Expression.Call(collectionProp, "StartsWith", Type.EmptyTypes, constant);
                        else if (isMin) // Valor mínimo
                            condition = Expression.GreaterThanOrEqual(collectionProp, constant);
                        else if (isMax) // Valor máximo
                            condition = Expression.LessThanOrEqual(collectionProp, constant);
                        else // Igualdade normal
                            condition = Expression.Equal(collectionProp, constant);

                        // Se houver múltiplos valores no mesmo campo, aplicar OR entre eles
                        body = body == null ? condition : Expression.OrElse(body, condition);
                    }

                    var anyLambda = Expression.Lambda(body ?? Expression.Constant(true), collectionParam);
                    return Expression.Lambda<Func<T, bool>>(
                        Expression.Call(typeof(Enumerable), "Any", new Type[] { elementType }, Expression.Property(param, propInfo.Name), anyLambda),
                        param
                    );
                }
            }
            else
            {
                // **Caminho normal para acessar propriedades diretas**
                prop = Expression.Property(prop, propInfo.Name);
                currentType = propInfo.PropertyType; // **Atualiza para a nova propriedade**
            }
        }

        throw new InvalidOperationException("A estrutura do filtro não foi reconhecida.");
    }

    /// <summary>
    /// Verifica se um produto está presente em algum carrinho ativo.
    /// </summary>
    /// <param name="productId">O ID do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Verdadeiro se o produto estiver em um carrinho, falso caso contrário.</returns>
    public async Task<bool> IsProductInAnyCartAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .AnyAsync(c => c.Items.Any(i => i.ProductId == productId), cancellationToken);
    }


}
