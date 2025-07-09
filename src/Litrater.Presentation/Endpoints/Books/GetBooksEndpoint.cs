using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Application.Features.Books.Queries.GetBooks;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Books;

internal sealed class GetBooksEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("books",
                async (IQueryHandler<GetBooksQuery, PagedResult<IEnumerable<BookDto>>> handler, CancellationToken cancellationToken,
                    [FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
                {
                    var query = new GetBooksQuery(page, pageSize);

                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("GetBooks")
            .WithTags("Books")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<PagedResult<IEnumerable<BookDto>>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }
}