using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Features.Books.Queries.GetBooks;

internal sealed class GetBooksQueryHandler(IBookRepository bookRepository) : IQueryHandler<GetBooksQuery, PagedResult<IEnumerable<BookDto>>>
{
    public async Task<Result<PagedResult<IEnumerable<BookDto>>>> Handle(GetBooksQuery query, CancellationToken cancellationToken)
    {
        var (books, totalCount) = await bookRepository.GetBooksAsync(query.Page, query.PageSize, cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var pagedInfo = new PagedInfo(query.Page, query.PageSize, totalPages, totalCount);

        return new PagedResult<IEnumerable<BookDto>>(pagedInfo, books.Select(book => book.ToDto()));
    }
}