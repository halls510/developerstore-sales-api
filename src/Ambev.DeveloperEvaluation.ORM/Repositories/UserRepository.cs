using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of IUserRepository using Entity Framework Core
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of UserRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public UserRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checks if a user exists by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the user exists, false otherwise</returns>
    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Creates a new user in the database
    /// </summary>
    /// <param name="user">The user to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created user</returns>
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity; // Retorna o objeto com ID gerado
    }

    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their email address
    /// </summary>
    /// <param name="email">The email address to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
                      .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="user">The user with updated information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user.</returns>
    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var existingUser = await GetByIdAsync(user.Id, cancellationToken);
        if (existingUser == null)
            throw new ResourceNotFoundException("User not found", $"User with ID {user.Id} not found.");

        _context.Entry(existingUser).CurrentValues.SetValues(user);
        await _context.SaveChangesAsync(cancellationToken);
        return existingUser;
    }

    /// <summary>
    /// Deletes a user from the database
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the user was deleted, false if not found</returns>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(id, cancellationToken);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves a paginated list of users with optional sorting.
    /// </summary>
    public async Task<List<User>> GetUsersAsync(int page, int size, string? orderBy, Dictionary<string, string[]>? filters, CancellationToken cancellationToken)
    {
        var query = _context.Users
                        .Include(p => p.Address)
                        .ThenInclude(a => a.Geolocation)
                        .AsQueryable();

        filters = CleanFilters(filters);

        // Aplica filtros
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(BuildPredicate<User>(filter.Key, filter.Value));
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
    private IQueryable<User> ApplySorting(IQueryable<User> query, string orderBy)
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
    private IQueryable<User> ApplyOrder(IQueryable<User> query, string propertyPath, bool isAscending)
    {
        var param = Expression.Parameter(typeof(User), "p");
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
            new Type[] { typeof(User), property.Type },
            query.Expression,
            Expression.Quote(lambda)
        );

        return query.Provider.CreateQuery<User>(orderByExpression);
    }

    /// <summary>
    /// Retrieves the total count of Users in the database.
    /// </summary>
    public async Task<int> CountUsersAsync(Dictionary<string, string[]>? filters, CancellationToken cancellationToken)
    {
        var query = _context.Users
                        .Include(p => p.Address)
                        .ThenInclude(a => a.Geolocation)
                        .AsQueryable();

        filters = CleanFilters(filters);

        // Aplica filtros
        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                query = query.Where(BuildPredicate<User>(filter.Key, filter.Value));
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
    private Dictionary<string, string[]> CleanFilters(Dictionary<string, string[]>? filters)
    {
        if (filters == null) return new Dictionary<string, string[]>();

        var cleanedFilters = filters
            .Where(kvp => kvp.Key != "_page" && kvp.Key != "_size" && kvp.Key != "_order")
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return cleanedFilters;
    }

}
