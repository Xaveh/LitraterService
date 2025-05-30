using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id) : IQuery;