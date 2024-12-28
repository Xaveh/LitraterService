namespace Litrater.Domain.Models;

public sealed class Review : Entity
{
    public Review(Guid id, string content, int rating, Guid bookId) : base(id)
    {
        Content = content ?? throw new ArgumentNullException(nameof(content));
        Rating = rating;
        BookId = bookId;
    }

    public string Content { get; private set; }
    public int Rating { get; private set; }
    public Guid BookId { get; private set; }
}