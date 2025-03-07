using Ambev.DeveloperEvaluation.Domain.Entities;
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
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }

    /// <summary>
    /// Retrieves a user by their unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(o=> o.Id == id, cancellationToken);
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
            throw new KeyNotFoundException("User not found.");

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
    /// <param name="page">The page number for pagination (starting from 1).</param>
    /// <param name="size">The number of users per page.</param>
    /// <param name="orderBy">
    /// Sorting criteria in the format "field asc" or "field desc". 
    /// Multiple fields can be separated by commas (e.g., "username asc, email desc").
    /// </param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>A paginated list of users.</returns>
    public async Task<List<User>> GetUsersAsync(int page, int size, string? orderBy, CancellationToken cancellationToken)
    {
        var query = _context.Users.AsQueryable();

        // Aplica ordenação se fornecida
        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = ApplySorting(query, orderBy);
        }
        else
        {
            // Ordenação padrão por ID ascendente
            query = query.OrderBy(u => u.Id);
        }

        return await query
            .Skip((page - 1) * size) // Pula os registros das páginas anteriores
            .Take(size) // Retorna apenas os registros da página atual
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Aplica ordenação dinâmica baseada em uma string no formato "campo asc, campo2 desc".
    /// </summary>
    /// <param name="query">Consulta base.</param>
    /// <param name="orderBy">String de ordenação.</param>
    /// <returns>Consulta ordenada.</returns>
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
    /// Aplica ordenação genérica a um IQueryable, incluindo propriedades aninhadas.
    /// </summary>
    /// <param name="query">Consulta base.</param>
    /// <param name="propertyPath">Nome da propriedade para ordenar (suporta propriedades aninhadas, como "Address.City").</param>
    /// <param name="isAscending">Direção da ordenação.</param>
    /// <returns>Consulta ordenada.</returns>
    private IQueryable<User> ApplyOrder(IQueryable<User> query, string propertyPath, bool isAscending)
    {
        var param = Expression.Parameter(typeof(User), "u");
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
    /// Obtém a contagem total de usuários no banco de dados.
    /// </summary>
    public async Task<int> CountUsersAsync(CancellationToken cancellationToken)
    {
        return await _context.Users.CountAsync(cancellationToken);
    }
}
