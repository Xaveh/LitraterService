namespace Litrater.Domain.Models;

public sealed class Book : Entity
{
    private readonly List<Author> _authors = [];
    private readonly List<Review> _reviews = [];

    public Book(Guid id, string title, string isbn) : base(id)
    {
        Title = title;
        Isbn = isbn;
    }

    public string Title { get; private set; }
    public string Isbn { get; private set; }
    public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();
    public IReadOnlyCollection<Review> Reviews => _reviews.AsReadOnly();

    public void AddAuthor(Author author)
    {
        if (!_authors.Contains(author))
            _authors.Add(author);
    }

    public void AddReview(Review review)
    {
        if (!_reviews.Contains(review))
            _reviews.Add(review);
    }
}