using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface IUserService
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}

