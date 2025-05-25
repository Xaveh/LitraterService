using Litrater.Domain.Books;

namespace Litrater.Application.Books.Dtos;

public static class BookDtoExtensions
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto(
            Id: book.Id,
            Title: book.Title,
            Isbn: book.Isbn,
            AuthorIds: book.Authors.Select(a => a.Id),
            ReviewIds: book.Reviews.Select(r => r.Id)
        );
    }
} 