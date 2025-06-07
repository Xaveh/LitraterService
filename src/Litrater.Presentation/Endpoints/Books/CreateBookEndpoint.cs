using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Commands.CreateBook;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;

namespace Litrater.Presentation.Endpoints.Books;

internal sealed class CreateBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("books",
                async (CreateBookRequest request, ICommandHandler<CreateBookCommand, BookDto> handler, CancellationToken cancellationToken) =>
                {
                    var command = new CreateBookCommand(
                        Title: request.Title,
                        Isbn: request.Isbn,
                        AuthorIds: request.AuthorIds
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("CreateBook")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<BookDto>(StatusCodes.Status200OK)
            .Produces<ValidationError[]>(StatusCodes.Status400BadRequest);
    }
}

internal sealed record CreateBookRequest(
    string Title,
    string Isbn,
    IEnumerable<Guid> AuthorIds
);
