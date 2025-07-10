using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

public sealed record DeleteBookReviewCommand(Guid Id, Guid UserId, bool IsAdmin = false) : ICommand; 