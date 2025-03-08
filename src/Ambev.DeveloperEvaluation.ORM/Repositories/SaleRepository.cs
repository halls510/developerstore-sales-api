using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
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
        var existingSale = await GetByIdAsync(sale.Id, cancellationToken);
        if (existingSale == null)
            throw new KeyNotFoundException("Sale not found.");

        _context.Entry(existingSale).CurrentValues.SetValues(sale);
        existingSale.Items = sale.Items;
        await _context.SaveChangesAsync(cancellationToken);
        return existingSale;
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
}
