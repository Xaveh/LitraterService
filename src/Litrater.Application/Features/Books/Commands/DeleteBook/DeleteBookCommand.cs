using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(Guid Id) : ICommand;