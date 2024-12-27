namespace Litrater.Domain.Models;

public sealed class Review
{
    public Review(Guid id, string content, int rating, Guid bookId)
    {
        Id = id;
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Rating = rating;
        BookId = bookId;
    }

    public Guid Id { get; private set; }
    public string Content { get; private set; }
    public int Rating { get; private set; }
    public Guid BookId { get; private set; }
}