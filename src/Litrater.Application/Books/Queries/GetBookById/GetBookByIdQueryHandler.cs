using Ardalis.Result;
using Litrater.Application.Books.Dtos;
using Litrater.Application.Books.Interfaces;
using Litrater.Application.Common.Interfaces;

namespace Litrater.Application.Books.Queries.GetBookById;

internal sealed class GetBookByIdQueryHandler(IBookRepository bookRepository) : IQueryHandler<GetBookByIdQuery, BookDto>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken) 
    {
        var book = await bookRepository.GetByIdAsync(query.Id, cancellationToken);

        if (book is null)
        {
            return Result<BookDto>.NotFound();
        }

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Isbn = book.Isbn,
            AuthorIds = book.Authors.Select(a => a.Id),
            ReviewIds = book.Reviews.Select(r => r.Id)
        };
    }
}
