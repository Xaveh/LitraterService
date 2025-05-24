using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.AspNetCore;
using Litrater.Application.Books.Dtos;
using Litrater.Application.Books.Queries.GetBookById;
using Litrater.Application.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Litrater.Presentation.Books.GetBookById;

public static class GetBookByIdEndpoint
{
    public static void MapGetBookByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/books/{id:guid}",
                async (Guid id, IQueryHandler<GetBookByIdQuery, BookDto> handler, CancellationToken cancellationToken) =>
                {
                    var query = new GetBookByIdQuery(id);
                    var result =  await handler.Handle(query, cancellationToken);
                    
                    return result.ToMinimalApiResult();
                })
            .WithName("GetBookById")
            .WithOpenApi()
            .Produces<BookDto>(StatusCodes.Status200OK)
            .Produces<ValidationError[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
}
