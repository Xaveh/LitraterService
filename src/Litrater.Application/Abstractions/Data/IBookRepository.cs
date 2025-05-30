using Litrater.Domain.Books;

namespace Litrater.Application.Abstractions.Data;

public interface IBookRepository : IRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}