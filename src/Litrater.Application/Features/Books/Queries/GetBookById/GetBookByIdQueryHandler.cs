using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Features.Books.Queries.GetBookById;

internal sealed class GetBookByIdQueryHandler(IBookQueryRepository bookQueryRepository) : IQueryHandler<GetBookByIdQuery, BookDto>
{
    public async Task<Result<BookDto>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken)
    {
        var book = await bookQueryRepository.GetByIdAsync(query.Id, cancellationToken);

        return book is null ? Result<BookDto>.NotFound() : Result<BookDto>.Success(book);
    }
}