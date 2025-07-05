using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

internal abstract class Repository<T>(LitraterDbContext context) : IRepository
    where T : Entity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();
}