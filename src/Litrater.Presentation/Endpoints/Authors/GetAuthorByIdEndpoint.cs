using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Application.Features.Authors.Queries.GetAuthorById;
using Litrater.Presentation.Abstractions;
using Litrater.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Litrater.Presentation.Endpoints.Authors;

internal sealed class GetAuthorByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("authors/{id:guid}",
                async (Guid id, IQueryHandler<GetAuthorByIdQuery, AuthorDto> handler, CancellationToken cancellationToken) =>
                {
                    var query = new GetAuthorByIdQuery(id);
                    var result = await handler.Handle(query, cancellationToken);

                    return result.ToHttpResult();
                })
            .WithName("GetAuthorById")
            .WithTags("Authors")
            .MapToApiVersion(1)
            .WithOpenApi()
            .Produces<AuthorDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
} 