using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.EventHandlers;
using Litrater.Domain.Books;
using Litrater.Domain.Books.Events;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class BookReviewRatingChangedDomainEventHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly BookReviewRatingChangedDomainEventHandler _handler;

    public BookReviewRatingChangedDomainEventHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _handler = new BookReviewRatingChangedDomainEventHandler(_bookCommandRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_AddReview_WhenNoExistingReviews_ShouldSetAverageToNewRating()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var domainEvent = new BookReviewRatingChangedDomainEvent(bookId, null, 4);

        _bookCommandRepositoryMock
            .Setup(x => x.GetReviewsAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _bookCommandRepositoryMock.Verify(
            x => x.SetAverageRatingAsync(bookId, 4.0, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_AddReview_WhenExistingReviews_ShouldCalculateCorrectAverage()
    {
        // Arrange: existing avg = 4.0 with 2 reviews (4, 4), adding rating 1 → (4+4+1)/3 = 3.0
        var bookId = Guid.NewGuid();
        var domainEvent = new BookReviewRatingChangedDomainEvent(bookId, null, 1);

        _bookCommandRepositoryMock
            .Setup(x => x.GetReviewsAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Rating(4), new Rating(4)]);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _bookCommandRepositoryMock.Verify(
            x => x.SetAverageRatingAsync(bookId, 3.0, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateReview_ShouldCalculateCorrectAverage()
    {
        // Arrange: existing avg = 4.0 with 2 reviews (5, 3), updating from 5 to 3 → (3+3)/2 = 3.0
        var bookId = Guid.NewGuid();
        var domainEvent = new BookReviewRatingChangedDomainEvent(bookId, 5, 3);

        _bookCommandRepositoryMock
            .Setup(x => x.GetReviewsAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Rating(5), new Rating(3)]);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _bookCommandRepositoryMock.Verify(
            x => x.SetAverageRatingAsync(bookId, 3.0, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RemoveReview_WhenMultipleReviews_ShouldCalculateCorrectAverage()
    {
        // Arrange: existing avg = 4.0 with 3 reviews (5, 4, 3), removing rating 5 → (4+3)/2 = 3.5
        var bookId = Guid.NewGuid();
        var domainEvent = new BookReviewRatingChangedDomainEvent(bookId, 5, null);

        _bookCommandRepositoryMock
            .Setup(x => x.GetReviewsAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Rating(5), new Rating(4), new Rating(3)]);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _bookCommandRepositoryMock.Verify(
            x => x.SetAverageRatingAsync(bookId, 3.5, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_RemoveReview_WhenLastReview_ShouldSetAverageToNull()
    {
        // Arrange: existing avg = 5.0 with 1 review (5), removing it → null
        var bookId = Guid.NewGuid();
        var domainEvent = new BookReviewRatingChangedDomainEvent(bookId, 5, null);

        _bookCommandRepositoryMock
            .Setup(x => x.GetReviewsAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new Rating(5)]);

        // Act
        await _handler.Handle(domainEvent, CancellationToken.None);

        // Assert
        _bookCommandRepositoryMock.Verify(
            x => x.SetAverageRatingAsync(bookId, null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void AddReview_ShouldRaiseDomainEventWithNewRating()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);

        // Act
        book.AddReview(Guid.NewGuid(), "Great!", new Rating(5), Guid.NewGuid());

        // Assert
        book.DomainEvents.Count.ShouldBe(1);
        var domainEvent = book.DomainEvents[0].ShouldBeOfType<BookReviewRatingChangedDomainEvent>();
        domainEvent.BookId.ShouldBe(bookId);
        domainEvent.OldRating.ShouldBeNull();
        domainEvent.NewRating.ShouldBe(5);
    }

    [Fact]
    public void UpdateReview_WhenRatingChanged_ShouldRaiseDomainEventWithOldAndNewRating()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Good!", new Rating(3), userId)!;
        book.ClearDomainEvents();

        // Act
        book.UpdateReview(review.Id, "Even better!", new Rating(5));

        // Assert
        book.DomainEvents.Count.ShouldBe(1);
        var domainEvent = book.DomainEvents[0].ShouldBeOfType<BookReviewRatingChangedDomainEvent>();
        domainEvent.BookId.ShouldBe(bookId);
        domainEvent.OldRating.ShouldBe(3);
        domainEvent.NewRating.ShouldBe(5);
    }

    [Fact]
    public void UpdateReview_WhenRatingNotChanged_ShouldNotRaiseDomainEvent()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Good!", new Rating(3), userId)!;
        book.ClearDomainEvents();

        // Act
        book.UpdateReview(review.Id, "Updated content only", new Rating(3));

        // Assert
        book.DomainEvents.ShouldBeEmpty();
    }

    [Fact]
    public void RemoveReview_ShouldRaiseDomainEventWithOldRating()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Good!", new Rating(4), userId)!;
        book.ClearDomainEvents();

        // Act
        book.RemoveReview(review.Id);

        // Assert
        book.DomainEvents.Count.ShouldBe(1);
        var domainEvent = book.DomainEvents[0].ShouldBeOfType<BookReviewRatingChangedDomainEvent>();
        domainEvent.BookId.ShouldBe(bookId);
        domainEvent.OldRating.ShouldBe(4);
        domainEvent.NewRating.ShouldBeNull();
    }
}