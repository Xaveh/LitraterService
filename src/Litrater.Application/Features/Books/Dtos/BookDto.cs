namespace Litrater.Application.Features.Books.Dtos;

public record BookDto(
    Guid Id,
    string Title,
    string Isbn,
    IEnumerable<Guid> AuthorIds,
    IEnumerable<Guid> ReviewIds
);