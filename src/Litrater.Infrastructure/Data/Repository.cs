using Litrater.Application.Common.Interfaces;
using Litrater.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

public abstract class Repository<T>(LitraterDbContext context) : IRepository
    where T : Entity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();
}