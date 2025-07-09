using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Authors.Commands.DeleteAuthor;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Authors;

internal sealed class DeleteAuthorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("authors/{id:guid}",
                async (Guid id, ICommandHandler<DeleteAuthorCommand> handler, CancellationToken cancellationToken) =>
                {
                    var command = new DeleteAuthorCommand(id);

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("DeleteAuthor")
            .WithTags("Authors")
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