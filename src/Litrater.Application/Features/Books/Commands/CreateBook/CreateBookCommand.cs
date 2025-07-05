using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.CreateBook;

public record CreateBookCommand(string Title, string Isbn, IEnumerable<Guid> AuthorIds) : ICommand;