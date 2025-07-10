using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Commands.CreateBookReview;

public sealed record CreateBookReviewCommand(string Content, int Rating, Guid BookId, Guid UserId) : ICommand; 