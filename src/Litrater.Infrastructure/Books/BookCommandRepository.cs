using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookCommandRepository(LitraterDbContext context) : CommandRepository<Book>(context), IBookCommandRepository
{
    private readonly DbSet<BookReview> _bookReviews = context.Set<BookReview>();

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

    public Task<List<Rating>> GetReviewsAsync(
        Guid bookId, CancellationToken cancellationToken = default)
    {
        return _bookReviews
            .Where(r => r.BookId == bookId)
            .Select(r => r.Rating)
            .ToListAsync(cancellationToken);
    }

    public async Task SetAverageRatingAsync(Guid bookId, double? averageRating, CancellationToken cancellationToken = default)
    {
        var book = await DbSet.FindAsync([bookId], cancellationToken);
        book!.UpdateAverageRating(averageRating);
    }

    protected override IQueryable<Book> ApplyIncludes(IQueryable<Book> query)
    {
        return query.Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery();
    }
}