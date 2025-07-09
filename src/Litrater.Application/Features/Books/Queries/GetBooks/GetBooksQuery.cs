using Litrater.Application.Abstractions.CQRS;

namespace Litrater.Application.Features.Books.Queries.GetBooks;

public record GetBooksQuery(int Page = 1, int PageSize = 10) : IQuery;