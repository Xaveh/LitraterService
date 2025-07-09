using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.CreateBook;

public sealed record CreateBookCommand(string Title, string Isbn, IEnumerable<Guid> AuthorIds) : ICommand;