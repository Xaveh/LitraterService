using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Dtos;

public static class BookReviewDtoExtensions
{
    public static BookReviewDto ToDto(this BookReview bookReview)
    {
        return new BookReviewDto(
            Id: bookReview.Id,
            Content: bookReview.Content,
            Rating: bookReview.Rating,
            BookId: bookReview.BookId,
            UserId: bookReview.UserId
        );
    }
} 