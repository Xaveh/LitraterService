using Litrater.Application.Common.Interfaces;
using Litrater.Domain.Books;

namespace Litrater.Application.Books.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
