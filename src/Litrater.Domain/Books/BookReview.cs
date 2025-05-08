using Litrater.Domain.Shared;

namespace Litrater.Domain.Books;

public sealed class BookReview : Entity
{
    #pragma warning disable CS8618 // Required by Entity Framework
    private BookReview() {}
    
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