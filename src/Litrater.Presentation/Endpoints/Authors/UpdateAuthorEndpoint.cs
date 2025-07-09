using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Authors.Commands.UpdateAuthor;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Authors;

internal sealed class UpdateAuthorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("authors/{id:guid}",
                async (Guid id, UpdateAuthorRequest request, ICommandHandler<UpdateAuthorCommand, AuthorDto> handler, CancellationToken cancellationToken) =>
                {
                    var command = new UpdateAuthorCommand(
                        Id: id,
                        FirstName: request.FirstName,
                        LastName: request.LastName,
                        BookIds: request.BookIds
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("UpdateAuthor")
            .WithTags("Authors")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);
    }
}

internal sealed record UpdateAuthorRequest(
    string FirstName,
    string LastName,
    IEnumerable<Guid> BookIds
); 