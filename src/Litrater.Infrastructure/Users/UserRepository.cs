using Litrater.Application.Abstractions.Authentication;
using Litrater.Domain.Users;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Users;

internal sealed class UserRepository(LitraterDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByKeycloakUserIdAsync(string keycloakUserId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.KeycloakUserId == keycloakUserId, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(user, cancellationToken);
    }
}