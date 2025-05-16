using Litrater.Application.Common.Interfaces;
using Litrater.Domain.Shared;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly DbSet<T> DbSet;

    protected Repository(LitraterDbContext context)
    {
        DbSet = context.Set<T>();
    }
}