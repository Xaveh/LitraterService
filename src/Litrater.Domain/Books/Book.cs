using Litrater.Domain.Authors;
using Litrater.Domain.Common;

namespace Litrater.Domain.Books;

public sealed class Book : AggregateRoot
{
    private readonly List<Author> _authors;
    private readonly List<BookReview> _reviews = [];

#pragma warning disable CS8618 // Required by Entity Framework
    private Book() { }

    public Book(Guid id, string title, Isbn isbn, List<Author> authors) : base(id)
    {
        Title = title;
        Isbn = isbn;
        _authors = authors;
    }

    public string Title { get; private set; }
    public Isbn Isbn { get; private set; }
    public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();
    public IReadOnlyCollection<BookReview> Reviews => _reviews.AsReadOnly();

    public void Update(string title, Isbn isbn, List<Author> authors)
    {
        Title = title;
        Isbn = isbn;
        _authors.Clear();
        _authors.AddRange(authors);
    }

    public BookReview? AddReview(Guid id, string content, Rating rating, Guid userId)
    {
        if (_reviews.Any(r => r.UserId == userId))
        {
            return null;
        }

        var review = new BookReview(id, content, rating, Id, userId);
        _reviews.Add(review);
        return review;
    }

    public BookReview? FindReview(Guid reviewId)
    {
        return _reviews.FirstOrDefault(r => r.Id == reviewId);
    }

    public void UpdateReview(Guid reviewId, string content, Rating rating)
    {
        var review = _reviews.First(r => r.Id == reviewId);
        review.Update(content, rating);
    }

    public void RemoveReview(Guid reviewId)
    {
        var review = _reviews.First(r => r.Id == reviewId);
        _reviews.Remove(review);
    }
}