using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Commands;
using Litrater.Application.Features.Books.Commands.UpdateBook;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Books;

internal sealed class UpdateBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("books/{id:guid}",
                async (Guid id, UpdateBookRequest request, ICommandHandler<UpdateBookCommand, BookDto> handler, CancellationToken cancellationToken) =>
                {
                    var command = new UpdateBookCommand(
                        Id: id,
                        Title: request.Title,
                        Isbn: request.Isbn,
                        AuthorIds: request.AuthorIds
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("UpdateBook")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<BookDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);
    }
}

internal sealed record UpdateBookRequest(
    string Title,
    string Isbn,
    IEnumerable<Guid> AuthorIds
);