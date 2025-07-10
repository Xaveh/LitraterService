using System.Security.Claims;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Books.Commands.CreateBookReview;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.BookReviews;

internal sealed class CreateBookReviewEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("books/{bookId:guid}/reviews",
                async (Guid bookId, CreateBookReviewRequest request, ClaimsPrincipal user, ICommandHandler<CreateBookReviewCommand, BookReviewDto> handler, CancellationToken cancellationToken) =>
                {
                    var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!Guid.TryParse(userIdClaim, out var userId))
                    {
                        return Results.Unauthorized();
                    }

                    var command = new CreateBookReviewCommand(
                        Content: request.Content,
                        Rating: request.Rating,
                        BookId: bookId,
                        UserId: userId
                    );

                    var result = await handler.Handle(command, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("CreateBookReview")
            .WithTags("BookReviews")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<BookReviewDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status409Conflict)
            .RequireAuthorization(AuthorizationPolicies.UserOrAdmin);
    }
}

internal sealed record CreateBookReviewRequest(string Content, int Rating);