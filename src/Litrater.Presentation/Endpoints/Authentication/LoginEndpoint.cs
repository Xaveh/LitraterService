using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Authentication.Queries.Login;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Authentication;

internal sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login",
                async (LoginRequest request, IQueryHandler<LoginQuery, string> handler, CancellationToken cancellationToken) =>
                {
                    var query = new LoginQuery(request.Email, request.Password);

                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("Login")
            .WithTags("Authentication")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<string>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .AllowAnonymous();
    }
}

internal sealed record LoginRequest(string Email, string Password);