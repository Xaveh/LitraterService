using Litrater.Application.Abstractions.Authentication;
using Litrater.Domain.Users;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Users;

internal sealed class UserRepository(LitraterDbContext context) : CommandRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByKeycloakUserIdAsync(Guid keycloakUserId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakUserId, cancellationToken);
    }
}