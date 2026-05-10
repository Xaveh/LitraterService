using System.Linq.Expressions;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Dtos;

public static class BookDtoExtensions
{
    public static readonly Expression<Func<Book, BookDto>> Projection =
        book => new BookDto(
            Id: book.Id,
            Title: book.Title,
            Isbn: book.Isbn.Value,
            AuthorIds: book.Authors.Select(a => a.Id),
            ReviewIds: book.Reviews.Select(r => r.Id)
        );

    private static readonly Func<Book, BookDto> CompiledProjection = Projection.Compile();

    public static BookDto ToDto(this Book book)
    {
        return CompiledProjection(book);
    }
}