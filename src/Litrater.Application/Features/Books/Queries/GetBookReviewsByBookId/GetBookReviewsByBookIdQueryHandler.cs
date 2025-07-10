using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;

namespace Litrater.Application.Features.Books.Queries.GetBookReviewsByBookId;

internal sealed class GetBookReviewsByBookIdQueryHandler(IBookReviewRepository bookReviewRepository) : IQueryHandler<GetBookReviewsByBookIdQuery, PagedResult<IEnumerable<BookReviewDto>>>
{
    public async Task<Result<PagedResult<IEnumerable<BookReviewDto>>>> Handle(GetBookReviewsByBookIdQuery query, CancellationToken cancellationToken)
    {
        var (reviews, totalCount) = await bookReviewRepository.GetBookReviewsByBookIdAsync(query.BookId, query.Page, query.PageSize, cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);
        var pagedInfo = new PagedInfo(query.Page, query.PageSize, totalPages, totalCount);

        return new PagedResult<IEnumerable<BookReviewDto>>(pagedInfo, reviews.Select(review => review.ToDto()));
    }
}