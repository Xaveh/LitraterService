namespace Litrater.Application.Features.Authors.Dtos;

public record AuthorDto(
    Guid Id,
    string FirstName,
    string LastName,
    IEnumerable<Guid> BookIds
);