using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Application.Features.Books.Queries.GetBookReviewsByUserId;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.BookReviews;

internal sealed class GetBookReviewsByUserIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/{userId:guid}/reviews",
                async (IQueryHandler<GetBookReviewsByUserIdQuery, PagedResult<IEnumerable<BookReviewDto>>> handler, CancellationToken cancellationToken,
                    Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
                {
                    var query = new GetBookReviewsByUserIdQuery(userId, page, pageSize);

                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("GetBookReviewsByUserId")
            .WithTags("BookReviews")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<PagedResult<IEnumerable<BookReviewDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
} 