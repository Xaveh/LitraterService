using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Authors.Commands.CreateAuthor;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Authors;

internal sealed class CreateAuthorEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("authors",
                async (CreateAuthorRequest request, ICommandHandler<CreateAuthorCommand, AuthorDto> handler, CancellationToken cancellationToken) =>
                {
                    var command = new CreateAuthorCommand(
                        FirstName: request.FirstName,
                        LastName: request.LastName
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("CreateAuthor")
            .WithTags("Authors")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(AuthorizationPolicies.AdminOnly);
    }
}

internal sealed record CreateAuthorRequest(string FirstName, string LastName);