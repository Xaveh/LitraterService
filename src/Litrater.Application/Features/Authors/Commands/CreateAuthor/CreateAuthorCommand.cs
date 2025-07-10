using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authors.Commands.CreateAuthor;

public sealed record CreateAuthorCommand(string FirstName, string LastName) : ICommand;