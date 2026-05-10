using System.Linq.Expressions;
using Litrater.Domain.Books;

namespace Litrater.Application.Features.Books.Dtos;

public static class BookReviewDtoExtensions
{
    public static readonly Expression<Func<BookReview, BookReviewDto>> Projection =
        review => new BookReviewDto(
            Id: review.Id,
            Content: review.Content,
            Rating: review.Rating.Value,
            BookId: review.BookId,
            UserId: review.UserId
        );

    private static readonly Func<BookReview, BookReviewDto> CompiledProjection = Projection.Compile();

    public static BookReviewDto ToDto(this BookReview bookReview)
    {
        return CompiledProjection(bookReview);
    }
}