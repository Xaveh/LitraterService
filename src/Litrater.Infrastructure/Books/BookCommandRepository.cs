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

    public Task<Book?> GetByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(b => b.Reviews.Where(r => r.Id == reviewId))
            .Where(b => b.Reviews.Any(r => r.Id == reviewId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    protected override IQueryable<Book> ApplyIncludes(IQueryable<Book> query)
    {
        return query.Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery();
    }
}