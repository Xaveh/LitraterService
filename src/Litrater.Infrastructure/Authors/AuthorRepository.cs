using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Authors;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Authors;

internal sealed class AuthorRepository(LitraterDbContext context) : Repository<Author>(context), IAuthorRepository
{
    public Task<List<Author>> GetAuthorsByIdsAsync(
        IEnumerable<Guid> authorIds,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(author => authorIds.Contains(author.Id))
            .ToListAsync(cancellationToken);
    }
}