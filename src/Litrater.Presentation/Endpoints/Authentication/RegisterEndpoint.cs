using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Authentication.Commands.Register;
using Litrater.Application.Features.Authentication.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Authentication;

internal sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register",
                async (RegisterRequest request, ICommandHandler<RegisterCommand, UserDto> handler, CancellationToken cancellationToken) =>
                {
                    var command = new RegisterCommand(
                        request.Email,
                        request.Password,
                        request.FirstName,
                        request.LastName
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("Register")
            .WithTags("Authentication")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status409Conflict)
            .AllowAnonymous();
    }
}

internal sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName);