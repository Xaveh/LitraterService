using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Users;

namespace Litrater.Application.Abstractions.Authentication;

public interface IUserRepository : IRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}