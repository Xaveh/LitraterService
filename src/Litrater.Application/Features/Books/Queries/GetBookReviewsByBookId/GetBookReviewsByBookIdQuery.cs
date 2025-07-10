using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Queries.GetBookReviewsByBookId;

public record GetBookReviewsByBookIdQuery(Guid BookId, int Page = 1, int PageSize = 10) : IQuery;