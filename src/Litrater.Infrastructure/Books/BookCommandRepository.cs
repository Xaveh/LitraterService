using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookCommandRepository(LitraterDbContext context) : CommandRepository<Book>(context), IBookCommandRepository
{
    public Task<List<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        return ApplyIncludes(DbSet)
            .Where(book => bookIds.Contains(book.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var isbnValue = new Isbn(isbn);
        return DbSet.AnyAsync(b => b.Isbn == isbnValue, cancellationToken);
    }

    protected override IQueryable<Book> ApplyIncludes(IQueryable<Book> query)
    {
        return query.Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery();
    }
}