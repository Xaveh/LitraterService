using Litrater.Domain.Books;

namespace Litrater.Application.Abstractions.Data;

public interface IBookReviewRepository : IRepository
{
    Task<BookReview?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<BookReview> Reviews, int TotalCount)> GetBookReviewsByBookIdAsync(Guid bookId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(BookReview bookReview, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default);
    void Delete(BookReview bookReview);
} 