using Ardalis.Result;
using Litrater.Application.Books.Dtos;
using Litrater.Application.Books.Interfaces;
using Litrater.Application.Common.Interfaces;

namespace Litrater.Application.Books.Queries.GetBookById;

public sealed class GetBookByIdQueryHandler : IQueryHandler<GetBookByIdQuery, BookDto>
{
    private readonly IBookRepository _bookRepository;

    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Result<BookDto>> Handle(GetBookByIdQuery query, CancellationToken cancellationToken) 
   
    {
        var book = await _bookRepository.GetByIdAsync(query.Id, cancellationToken);

        if (book == null)
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
