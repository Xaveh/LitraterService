using System.Linq.Expressions;
using Litrater.Domain.Authors;

namespace Litrater.Application.Features.Authors.Dtos;

public static class AuthorDtoExtensions
{
    public static readonly Expression<Func<Author, AuthorDto>> Projection =
        author => new AuthorDto(
            Id: author.Id,
            FirstName: author.Name.FirstName,
            LastName: author.Name.LastName,
            BookIds: author.Books.Select(b => b.Id)
        );

    private static readonly Func<Author, AuthorDto> CompiledProjection = Projection.Compile();

    public static AuthorDto ToDto(this Author author)
    {
        return CompiledProjection(author);
    }
}