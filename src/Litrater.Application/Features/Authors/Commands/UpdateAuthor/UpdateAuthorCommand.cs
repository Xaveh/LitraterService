using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authors.Commands.UpdateAuthor;

public sealed record UpdateAuthorCommand(
    Guid Id,
    string FirstName,
    string LastName,
    IEnumerable<Guid> BookIds
) : ICommand; 