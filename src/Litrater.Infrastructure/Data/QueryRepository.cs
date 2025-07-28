using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

internal abstract class QueryRepository<T>(LitraterDbContext context) : IQueryRepository
    where T : Entity
{
    protected readonly IQueryable<T> DbSet = context.Set<T>().AsNoTracking();
}