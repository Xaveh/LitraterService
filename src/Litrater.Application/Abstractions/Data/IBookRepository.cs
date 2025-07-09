using Litrater.Domain.Books;

namespace Litrater.Application.Abstractions.Data;

public interface IBookRepository : IRepository
{
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Book> Books, int TotalCount)> GetBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
    void Delete(Book book);
}