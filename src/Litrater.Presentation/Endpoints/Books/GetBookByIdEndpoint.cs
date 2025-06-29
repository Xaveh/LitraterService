using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Application.Features.Books.Queries.GetBookById;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Books;

internal sealed class GetBookByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("books/{id:guid}",
                async (Guid id, IQueryHandler<GetBookByIdQuery, BookDto> handler, CancellationToken cancellationToken) =>
                {
                    var query = new GetBookByIdQuery(id);
                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("GetBookById")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<BookDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}