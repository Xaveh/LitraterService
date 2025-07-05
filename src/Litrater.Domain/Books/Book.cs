using Litrater.Domain.Authors;
using Litrater.Domain.Common;

namespace Litrater.Domain.Books;

public sealed class Book : AggregateRoot
{
    private readonly List<Author> _authors;
    private readonly List<BookReview> _reviews = [];
    
    #pragma warning disable CS8618 // Required by Entity Framework
    private Book() {}
    
    public Book(Guid id, string title, string isbn, List<Author> authors) : base(id)
    {
        Title = title;
        Isbn = isbn;
        _authors = authors;
    }

    public string Title { get; private set; }
    public string Isbn { get; private set; }
    public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();
    public IReadOnlyCollection<BookReview> Reviews => _reviews.AsReadOnly();
    
    public void AddReview(BookReview bookReview)
    {
        if (!_reviews.Contains(bookReview))
        {
            _reviews.Add(bookReview);
        }
    }
}