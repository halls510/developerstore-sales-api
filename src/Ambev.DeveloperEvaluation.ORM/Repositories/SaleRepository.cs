using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checks if a sale exists by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales.AnyAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task<SaleItem?> GetSaleItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<SaleItem?> GetSaleItemByProductIdAsync(int saleId, int productId, CancellationToken cancellationToken = default)
    {
        return await _context.SaleItems
            .FirstOrDefaultAsync(item => item.SaleId == saleId && item.ProductId == productId, cancellationToken);
    }


    public async Task<Sale?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Sale>> GetAllAsync(int page, int pageSize, string orderBy, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.Include(s => s.Items).AsQueryable();

        if (!string.IsNullOrEmpty(orderBy))
        {
            query = orderBy.ToLower() switch
            {
                "date asc" => query.OrderBy(s => s.SaleDate),
                "date desc" => query.OrderByDescending(s => s.SaleDate),
                "totalvalue asc" => query.OrderBy(s => s.TotalValue),
                "totalvalue desc" => query.OrderByDescending(s => s.TotalValue),
                _ => query
            };
        }

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<SaleItem> UpdateItemAsync(SaleItem saleItem, CancellationToken cancellationToken = default)
    {
        _context.SaleItems.Update(saleItem);
        await _context.SaveChangesAsync(cancellationToken);
        return saleItem;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves a paginated list of sales with optional sorting.
    /// </summary>
    public async Task<List<Sale>> GetSalesAsync(int page, int size, string? orderBy, Dictionary<string, string[]>? filters, CancellationToken cancellationToken)
    {
        var query = _context.Sales.Include(p => p.Items).AsQueryable();

        filters = CleanFilters(filters);

        // Aplica filtros
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(BuildPredicate<Sale>(filter.Key, filter.Value));
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
    private IQueryable<Sale> ApplySorting(IQueryable<Sale> query, string orderBy)
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
    private IQueryable<Sale> ApplyOrder(IQueryable<Sale> query, string propertyPath, bool isAscending)
    {
        var param = Expression.Parameter(typeof(Sale), "p");
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
            new Type[] { typeof(Sale), property.Type },
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<Sale>(orderByExpression);
    }

    /// <summary>
    /// Retrieves the total count of Sales in the database.
    /// </summary>
    public async Task<int> CountSalesAsync(Dictionary<string, string[]>? filters, CancellationToken cancellationToken)
    {
        var query = _context.Sales.Include(p => p.Items).AsQueryable();

        filters = CleanFilters(filters);

        // Aplica filtros
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(BuildPredicate<Sale>(filter.Key, filter.Value));
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

            var propInfo = currentType.GetProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propPart, StringComparison.OrdinalIgnoreCase));

            if (propInfo == null)
            {
                throw new InvalidOperationException($"Propriedade '{propPart}' não encontrada em {currentType.Name}");
            }

            if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var elementType = propInfo.PropertyType.GetGenericArguments().First();
                var collectionParam = Expression.Parameter(elementType, "i");

                currentType = elementType;

                if (i + 1 < properties.Length)
                {
                    string nestedProperty = properties[i + 1];

                    var nestedPropInfo = currentType.GetProperties()
                        .FirstOrDefault(p => string.Equals(p.Name, nestedProperty, StringComparison.OrdinalIgnoreCase));

                    if (nestedPropInfo == null)
                    {
                        throw new InvalidOperationException($"Propriedade '{nestedProperty}' não encontrada em {currentType.Name}");
                    }

                    var collectionProp = Expression.Property(collectionParam, nestedPropInfo.Name);

                    Expression? body = null;

                    foreach (var val in values)
                    {
                        var trimmedValue = val.Trim('*');

                        object convertedValue;

                        if (prop.Type.IsEnum)
                        {
                            convertedValue = Enum.Parse(prop.Type, trimmedValue, ignoreCase: true);
                        }
                        else
                        {
                            convertedValue = Convert.ChangeType(trimmedValue, prop.Type);
                        }
                        var constant = Expression.Constant(convertedValue);
                        Expression condition;

                        if (collectionProp.Type == typeof(string))
                        {
                            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                            var propToLower = Expression.Call(collectionProp, toLowerMethod);
                            var constantToLower = Expression.Call(constant, toLowerMethod);

                            if (val.StartsWith("*") && val.EndsWith("*"))
                                condition = Expression.Call(propToLower, "Contains", Type.EmptyTypes, constantToLower);
                            else if (val.StartsWith("*"))
                                condition = Expression.Call(propToLower, "EndsWith", Type.EmptyTypes, constantToLower);
                            else if (val.EndsWith("*"))
                                condition = Expression.Call(propToLower, "StartsWith", Type.EmptyTypes, constantToLower);
                            else
                                condition = Expression.Equal(propToLower, constantToLower);
                        }
                        else if (isMin)
                            condition = Expression.GreaterThanOrEqual(collectionProp, constant);
                        else if (isMax)
                            condition = Expression.LessThanOrEqual(collectionProp, constant);
                        else
                            condition = Expression.Equal(collectionProp, constant);

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
                // Propriedade direta (não é lista)
                prop = Expression.Property(prop, propInfo.Name);
                currentType = propInfo.PropertyType;
            }
        }

        // === ADICIONADO: aplica o filtro para propriedades diretas ===
        Expression? finalBody = null;

        foreach (var val in values)
        {
            var trimmedValue = val.Trim('*');
            object convertedValue;

            if (prop.Type.IsEnum)
            {
                convertedValue = Enum.Parse(prop.Type, trimmedValue, ignoreCase: true);
            }
            else
            {
                convertedValue = Convert.ChangeType(trimmedValue, prop.Type);
            }
            var constant = Expression.Constant(convertedValue);
            Expression condition;

            if (prop.Type == typeof(string))
            {
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var propToLower = Expression.Call(prop, toLowerMethod);
                var constantToLower = Expression.Call(constant, toLowerMethod);

                if (val.StartsWith("*") && val.EndsWith("*"))
                    condition = Expression.Call(propToLower, "Contains", Type.EmptyTypes, constantToLower);
                else if (val.StartsWith("*"))
                    condition = Expression.Call(propToLower, "EndsWith", Type.EmptyTypes, constantToLower);
                else if (val.EndsWith("*"))
                    condition = Expression.Call(propToLower, "StartsWith", Type.EmptyTypes, constantToLower);
                else
                    condition = Expression.Equal(propToLower, constantToLower);
            }
            else if (isMin)
                condition = Expression.GreaterThanOrEqual(prop, constant);
            else if (isMax)
                condition = Expression.LessThanOrEqual(prop, constant);
            else
                condition = Expression.Equal(prop, constant);

            finalBody = finalBody == null ? condition : Expression.OrElse(finalBody, condition);
        }

        return Expression.Lambda<Func<T, bool>>(finalBody ?? Expression.Constant(true), param);
    }


    /// <summary>
    /// Verifica se um produto está presente em alguma venda ativa.
    /// </summary>
    /// <param name="productId">O ID do produto.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Verdadeiro se o produto estiver em uma venda, falso caso contrário.</returns>
    public async Task<bool> IsProductInAnySaleAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .AnyAsync(s => s.Items.Any(i => i.ProductId == productId), cancellationToken);
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
