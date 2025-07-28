using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Abstractions.Data;

public interface IBookReviewQueryRepository : IQueryRepository
{
    Task<(IEnumerable<BookReviewDto> Reviews, int TotalCount)> GetBookReviewsByBookIdAsync(Guid bookId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<(IEnumerable<BookReviewDto> Reviews, int TotalCount)> GetBookReviewsByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
}