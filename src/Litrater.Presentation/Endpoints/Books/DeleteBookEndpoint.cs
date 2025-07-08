using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Commands.DeleteBook;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Books;

internal sealed class DeleteBookEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("books/{id:guid}",
                async (Guid id, ICommandHandler<DeleteBookCommand> handler, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteBookCommand(id);

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("DeleteBook")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);
    }
}