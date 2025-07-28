using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Abstractions.Data;

public interface IBookQueryRepository : IQueryRepository
{
    Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<BookDto> Books, int TotalCount)> GetBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default);
}