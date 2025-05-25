using Litrater.Application.Common.Interfaces;

namespace Litrater.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id) : IQuery;
