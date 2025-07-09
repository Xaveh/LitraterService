using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookReviewRepository(LitraterDbContext dbContext) : Repository<BookReview>(dbContext), IBookReviewRepository
{
    public async Task<BookReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(br => br.Id == id, cancellationToken);
    }

    public async Task<(List<BookReview> Reviews, int TotalCount)> GetBookReviewsByBookIdAsync(Guid bookId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(br => br.BookId == bookId)
            .OrderByDescending(br => br.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }

    public async Task<(List<BookReview> Reviews, int TotalCount)> GetBookReviewsByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.CreatedDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var reviews = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }

    public async Task AddAsync(BookReview bookReview, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(bookReview, cancellationToken);
    }

    public async Task<bool> ExistsByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(br => br.UserId == userId && br.BookId == bookId, cancellationToken);
    }

    public void Delete(BookReview bookReview)
    {
        DbSet.Remove(bookReview);
    }
} 