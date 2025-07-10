using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.UpdateBookReview;

public sealed record UpdateBookReviewCommand(Guid Id, string Content, int Rating, Guid UserId, bool IsAdmin = false) : ICommand;