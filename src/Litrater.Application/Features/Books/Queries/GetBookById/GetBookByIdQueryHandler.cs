using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Features.Books.Queries.GetBookById;

internal sealed class GetBookByIdQueryHandler(IBookRepository bookRepository) : IQueryHandler<GetBookByIdQuery, BookDto>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(query.Id, cancellationToken);

        return book is null ? Result<BookDto>.NotFound() : book.ToDto();
    }
}