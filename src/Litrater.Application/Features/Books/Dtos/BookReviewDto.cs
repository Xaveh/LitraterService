namespace Litrater.Application.Features.Books.Dtos;

public record BookReviewDto(
    Guid Id,
    string Content,
    int Rating,
    Guid BookId,
    Guid UserId
); 