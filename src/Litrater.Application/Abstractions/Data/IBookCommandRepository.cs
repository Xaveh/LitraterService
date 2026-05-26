using Litrater.Domain.Books;

namespace Litrater.Application.Abstractions.Data;

public interface IBookCommandRepository : ICommandRepository<Book>
{
    Task<List<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    Task<Book?> GetByReviewIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<List<Rating>> GetReviewsAsync(Guid bookId, CancellationToken cancellationToken = default);
    Task SetAverageRatingAsync(Guid bookId, double? averageRating, CancellationToken cancellationToken = default);
}