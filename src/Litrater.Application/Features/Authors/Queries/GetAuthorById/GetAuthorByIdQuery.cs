using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Authors.Queries.GetAuthorById;

public record GetAuthorByIdQuery(Guid Id) : IQuery; 