using Litrater.Application.Abstractions.Common;
using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

public class UnitOfWork(LitraterDbContext context, IDateTimeProvider dateTimeProvider) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreatedDate(dateTimeProvider.UtcNow);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetModifiedDate(dateTimeProvider.UtcNow);
                    break;
            }
        }

        return await context.SaveChangesAsync(cancellationToken);
    }
}