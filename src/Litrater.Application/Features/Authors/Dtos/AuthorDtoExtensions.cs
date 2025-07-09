using Litrater.Domain.Authors;

namespace Litrater.Application.Features.Authors.Dtos;

public static class AuthorDtoExtensions
{
    public static AuthorDto ToDto(this Author author)
    {
        return new AuthorDto(
            Id: author.Id,
            FirstName: author.FirstName,
            LastName: author.LastName,
            BookIds: author.Books.Select(b => b.Id)
        );
    }
} 