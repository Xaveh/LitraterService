using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authors.Commands.DeleteAuthor;

public sealed record DeleteAuthorCommand(Guid Id) : ICommand; 