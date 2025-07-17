using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Users;

namespace Litrater.Application.Abstractions.Authentication;

public interface IUserRepository : IRepository
{
    Task<User?> GetByKeycloakUserIdAsync(Guid keycloakUserId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}