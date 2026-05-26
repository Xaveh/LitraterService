using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

internal abstract class CommandRepository<T>(LitraterDbContext context) : ICommandRepository<T>
    where T : Entity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await ApplyIncludes(DbSet).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual void Update(T entity)
    {
        context.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Delete(T entity)
    {
        DbSet.Remove(entity);
    }

    protected virtual IQueryable<T> ApplyIncludes(IQueryable<T> query)
    {
        return query;
    }
}