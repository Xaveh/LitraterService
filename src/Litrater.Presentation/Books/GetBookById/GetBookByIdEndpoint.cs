using System;
using System.Threading;
using System.Threading.Tasks;
using Litrater.Application.Books.Dtos;
using Litrater.Application.Books.Queries.GetBookById;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Litrater.Presentation.Books.GetBookById;

public static class GetBookByIdEndpoint
{
    public static void MapGetBookByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/books/{id}", async (Guid id, GetBookByIdQueryHandler handler, CancellationToken cancellationToken) =>
        {
            var query = new GetBookByIdQuery(id);
            var book = await handler.Handle(query, cancellationToken);

            if (book == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(book);
        })
        .WithName("GetBookById")
        .WithOpenApi()
        .Produces<BookDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
