using Litrater.Domain.Shared;

namespace Litrater.Domain.Models;

public sealed class BookReview : Entity
{
    public BookReview(Guid id, string content, int rating, Guid bookId) : base(id)
    {
        Content = content;
        Rating = rating;
        BookId = bookId;
    }

    public string Content { get; private set; }
    public int Rating { get; private set; }
    public Guid BookId { get; private set; }
}