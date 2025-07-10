using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Features.Books.Queries.GetBookReviewsByUserId;

internal sealed class GetBookReviewsByUserIdQueryHandler(IBookReviewRepository bookReviewRepository) : IQueryHandler<GetBookReviewsByUserIdQuery, PagedResult<IEnumerable<BookReviewDto>>>
{
    public async Task<Result<PagedResult<IEnumerable<BookReviewDto>>>> Handle(GetBookReviewsByUserIdQuery query, CancellationToken cancellationToken)
    {
        var (reviews, totalCount) = await bookReviewRepository.GetBookReviewsByUserIdAsync(query.UserId, query.Page, query.PageSize, cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var pagedInfo = new PagedInfo(query.Page, query.PageSize, totalPages, totalCount);

        return new PagedResult<IEnumerable<BookReviewDto>>(pagedInfo, reviews.Select(review => review.ToDto()));
    }
}