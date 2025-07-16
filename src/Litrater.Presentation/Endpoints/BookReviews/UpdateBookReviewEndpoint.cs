using System.Security.Claims;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Commands.UpdateBookReview;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.BookReviews;

internal sealed class UpdateBookReviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("book-reviews/{id:guid}",
                async (Guid id, UpdateBookReviewRequest request, ClaimsPrincipal user, ICommandHandler<UpdateBookReviewCommand, BookReviewDto> handler, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var isAdmin = user.HasClaim(ClaimTypes.Role, "admin");

                    var command = new UpdateBookReviewCommand(
                        Id: id,
                        Content: request.Content,
                        Rating: request.Rating,
                        UserId: userId,
                        IsAdmin: isAdmin
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("UpdateBookReview")
            .WithTags("BookReviews")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<BookReviewDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(AuthorizationPolicies.UserOrAdmin);
    }
}

internal sealed record UpdateBookReviewRequest(string Content, int Rating);