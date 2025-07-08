using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.UpdateBook;

public sealed record UpdateBookCommand(
    Guid Id,
    string Title,
    string Isbn,
    IEnumerable<Guid> AuthorIds
) : ICommand;