using Litrater.Domain.Books;

namespace Litrater.Application.Abstractions.Data;

public interface IBookCommandRepository : ICommandRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    void Delete(Book book);
}