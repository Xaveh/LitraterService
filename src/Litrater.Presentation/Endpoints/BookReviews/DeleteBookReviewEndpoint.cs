using System.Security.Claims;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Commands.DeleteBookReview;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.BookReviews;

internal sealed class DeleteBookReviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("book-reviews/{id:guid}",
                async (Guid id, ClaimsPrincipal user, ICommandHandler<DeleteBookReviewCommand> handler, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new DeleteBookReviewCommand(
                        Id: id,
                        UserId: userId,
                        IsAdmin: user.HasResourceRole("admin")
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("DeleteBookReview")
            .WithTags("BookReviews")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.UserOrAdmin);
    }
}