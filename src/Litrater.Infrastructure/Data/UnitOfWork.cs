using Litrater.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly LitraterDbContext _context;

    public UnitOfWork(LitraterDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
} 