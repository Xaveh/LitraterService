using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Application.Features.Books.Queries.GetBookReviewsByBookId;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.BookReviews;

internal sealed class GetBookReviewsByBookIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("books/{bookId:guid}/reviews",
                async (IQueryHandler<GetBookReviewsByBookIdQuery, PagedResult<IEnumerable<BookReviewDto>>> handler, CancellationToken cancellationToken,
                    Guid bookId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
                {
                    var query = new GetBookReviewsByBookIdQuery(bookId, page, pageSize);

                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("GetBookReviews")
            .WithTags("BookReviews")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<PagedResult<IEnumerable<BookReviewDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}