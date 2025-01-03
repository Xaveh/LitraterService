using Litrater.Domain.Shared;

namespace Litrater.Domain.Models;

public sealed class Book : AggregateRoot
{
    private readonly List<Author> _authors = [];
    private readonly List<BookReview> _reviews = [];

    public Book(Guid id, string title, string isbn) : base(id)
    {
        Title = title;
        Isbn = isbn;
    }

    public string Title { get; private set; }
    public string Isbn { get; private set; }
    public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();
    public IReadOnlyCollection<BookReview> Reviews => _reviews.AsReadOnly();

    public void AddAuthor(Author author)
    {
        if (!_authors.Contains(author))
        {
            _authors.Add(author);
        }
    }

    public void AddReview(BookReview bookReview)
    {
        if (!_reviews.Contains(bookReview))
        {
            _reviews.Add(bookReview);
        }
    }
}