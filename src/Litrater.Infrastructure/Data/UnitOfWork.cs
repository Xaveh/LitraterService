using Litrater.Application.Common.Interfaces;

namespace Litrater.Infrastructure.Data;

public class UnitOfWork(LitraterDbContext context) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
} 