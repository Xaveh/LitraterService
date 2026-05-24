using Litrater.Domain.Authors;
using Litrater.Domain.Books.Events;
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
    public double? AverageRating { get; private set; }
    public IReadOnlyCollection<Author> Authors => _authors.AsReadOnly();
    public IReadOnlyCollection<BookReview> Reviews => _reviews.AsReadOnly();

    public void Update(string title, Isbn isbn, List<Author> authors)
    {
        Title = title;
        Isbn = isbn;
        _authors.Clear();
        _authors.AddRange(authors);
    }

    public void UpdateAverageRating(double? averageRating)
    {
        AverageRating = averageRating;
    }

    public BookReview? AddReview(Guid id, string content, Rating rating, Guid userId)
    {
        if (_reviews.Any(r => r.UserId == userId))
        {
            return null;
        }

        var review = new BookReview(id, content, rating, Id, userId);
        _reviews.Add(review);
        RaiseDomainEvent(new BookReviewRatingChangedDomainEvent(Id, null, rating.Value));
        return review;
    }

    public BookReview? FindReview(Guid reviewId)
    {
        return _reviews.FirstOrDefault(r => r.Id == reviewId);
    }

    public void UpdateReview(Guid reviewId, string content, Rating rating)
    {
        var review = _reviews.First(r => r.Id == reviewId);
        var oldRating = review.Rating.Value;
        review.Update(content, rating);
        if (oldRating != rating.Value)
        {
            RaiseDomainEvent(new BookReviewRatingChangedDomainEvent(Id, oldRating, rating.Value));
        }
    }

    public void RemoveReview(Guid reviewId)
    {
        var review = _reviews.First(r => r.Id == reviewId);
        _reviews.Remove(review);
        RaiseDomainEvent(new BookReviewRatingChangedDomainEvent(Id, review.Rating.Value, null));
    }
}