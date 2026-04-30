using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Authors;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Authors;

internal sealed class AuthorCommandRepository(LitraterDbContext context) : CommandRepository<Author>(context), IAuthorCommandRepository
{
    public Task<List<Author>> GetAuthorsByIdsAsync(
        IEnumerable<Guid> authorIds,
        CancellationToken cancellationToken = default)
    {
        return DbSet.Where(author => authorIds.Contains(author.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(author => author.Name.FirstName == firstName && author.Name.LastName == lastName, cancellationToken);
    }

    protected override IQueryable<Author> ApplyIncludes(IQueryable<Author> query)
    {
        return query.Include(a => a.Books);
    }
}