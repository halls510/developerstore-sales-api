using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IProductService
{
    Task<Product?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);
}
