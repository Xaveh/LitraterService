using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Queries.GetBookReviewsByUserId;

public record GetBookReviewsByUserIdQuery(Guid UserId, int Page = 1, int PageSize = 10) : IQuery; 