using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookReviewQueryRepository(LitraterDbContext dbContext) : QueryRepository<BookReview>(dbContext), IBookReviewQueryRepository
{
    public async Task<(IEnumerable<BookReviewDto> Reviews, int TotalCount)> GetBookReviewsByBookIdAsync(Guid bookId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet
            .Where(br => br.BookId == bookId)
            .CountAsync(cancellationToken);

        var reviews = await DbSet
            .Where(br => br.BookId == bookId)
            .OrderByDescending(br => br.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(BookReviewDtoExtensions.Projection)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }

    public async Task<(IEnumerable<BookReviewDto> Reviews, int TotalCount)> GetBookReviewsByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet
            .Where(br => br.UserId == userId)
            .CountAsync(cancellationToken);

        var reviews = await DbSet
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(BookReviewDtoExtensions.Projection)
            .ToListAsync(cancellationToken);

        return (reviews, totalCount);
    }
}