using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Users;

namespace Litrater.Application.Abstractions.Authentication;

public interface IUserRepository : ICommandRepository<User>
{
    Task<User?> GetByKeycloakUserIdAsync(Guid keycloakUserId, CancellationToken cancellationToken = default);
}