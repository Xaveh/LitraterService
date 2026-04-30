using Litrater.Domain.Books;

namespace Litrater.Application.Abstractions.Data;

public interface IBookReviewCommandRepository : ICommandRepository<BookReview>
{
    Task<bool> ExistsByUserAndBookAsync(Guid userId, Guid bookId, CancellationToken cancellationToken = default);
}