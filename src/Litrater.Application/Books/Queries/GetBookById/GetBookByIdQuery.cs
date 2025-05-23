using Litrater.Application.Books.Dtos;
using Litrater.Application.Common.Interfaces;

namespace Litrater.Application.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id) : IQuery<BookDto>;
