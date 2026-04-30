using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Authors;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Authors;

internal sealed class AuthorCommandRepository(LitraterDbContext context) : CommandRepository<Author>(context), IAuthorCommandRepository
{
    public async Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

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

    public async Task AddAsync(Author author, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(author, cancellationToken);
    }

    public void Update(Author author)
    {
        Context.Entry(author).State = EntityState.Modified;
    }

    public void Delete(Author author)
    {
        DbSet.Remove(author);
    }
}