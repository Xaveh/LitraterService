using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Queries.GetBookReviews;

public record GetBookReviewsQuery(Guid BookId, int Page = 1, int PageSize = 10) : IQuery; 