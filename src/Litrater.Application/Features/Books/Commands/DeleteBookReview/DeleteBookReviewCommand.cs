using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

public sealed record DeleteBookReviewCommand(Guid Id, Guid KeycloakUserId, bool IsAdmin = false) : ICommand;