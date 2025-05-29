using Ardalis.Result;
using Litrater.Application.Books.Dtos;
using Litrater.Application.Books.Queries.GetBookById;
using Litrater.Application.Common.Interfaces;
using Litrater.Presentation.Common;
using Litrater.Presentation.Common.Interfaces;

namespace Litrater.Presentation.Endpoints.Books;

internal sealed class GetBookByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/books/{id:guid}",
                async (Guid id, IQueryHandler<GetBookByIdQuery, BookDto> handler, CancellationToken cancellationToken) =>
                {
                    var query = new GetBookByIdQuery(id);
                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("GetBookById")
            .WithOpenApi()
            .Produces<BookDto>(StatusCodes.Status200OK)
            .Produces<ValidationError[]>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);
    }
}