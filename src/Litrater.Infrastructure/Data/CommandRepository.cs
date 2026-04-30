using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

internal abstract class CommandRepository<T>(LitraterDbContext context) : ICommandRepository
    where T : Entity
{
    protected readonly LitraterDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();
}