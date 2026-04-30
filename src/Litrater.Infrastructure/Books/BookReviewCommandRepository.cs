using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookReviewCommandRepository(LitraterDbContext dbContext) : CommandRepository<BookReview>(dbContext), IBookReviewCommandRepository
{
    public async Task<bool> ExistsByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(br => br.UserId == userId && br.BookId == bookId, cancellationToken);
    }
}
